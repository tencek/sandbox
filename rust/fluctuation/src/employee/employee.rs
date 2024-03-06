use anyhow::anyhow;
use chrono::NaiveDate;
use csv::StringRecord;

#[derive(Debug)]
pub struct Employee {
    pub board_in: NaiveDate,
    pub board_out: Option<NaiveDate>,
}

impl Employee {
    pub fn new(board_in: NaiveDate, board_out: Option<NaiveDate>) -> Self {
        Employee {
            board_in,
            board_out,
        }
    }

    pub fn is_active(&self) -> bool {
        self.board_out.is_none()
    }

    pub fn was_active_by(&self, date: NaiveDate) -> bool {
        if let Some(board_out) = self.board_out {
            date >= self.board_in && date <= board_out
        } else {
            date >= self.board_in
        }
    }

    pub fn num_years_by(&self, date: NaiveDate) -> Option<f64> {
        if self.was_active_by(date) {
            Some(date.signed_duration_since(self.board_in).num_days() as f64 / 365.25)
        } else {
            None
        }
    }
}

impl TryFrom<StringRecord> for Employee {
    type Error = anyhow::Error;

    fn try_from(record: StringRecord) -> Result<Self, Self::Error> {
        let board_in_str = record
            .get(0)
            .ok_or(anyhow!("Invalid CSV record, 0th item missing"))?;
        let board_in = NaiveDate::parse_from_str(board_in_str, "%Y-%m-%d")?;
        let board_out_str = record
            .get(1)
            .ok_or(anyhow!("Invalid CSV record, 1st item missing"))?;

        let board_out = match board_out_str {
            "#N/A" => None,
            a_str => Some(NaiveDate::parse_from_str(a_str, "%Y-%m-%d")?),
        };
        Ok::<Employee, anyhow::Error>(Employee::new(board_in, board_out))
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use approx::*;
    use chrono::NaiveDate;

    #[test]
    fn test_was_active_at() {
        let employee = Employee::new(
            NaiveDate::from_ymd_opt(2020, 1, 1).unwrap(),
            Some(NaiveDate::from_ymd_opt(2020, 12, 31).unwrap()),
        );

        assert!(employee.was_active_by(NaiveDate::from_ymd_opt(2020, 1, 1).unwrap()));
        assert!(employee.was_active_by(NaiveDate::from_ymd_opt(2020, 12, 31).unwrap()));
        assert!(employee.was_active_by(NaiveDate::from_ymd_opt(2020, 6, 1).unwrap()));
        assert!(!employee.was_active_by(NaiveDate::from_ymd_opt(2021, 1, 1).unwrap()));
    }

    #[test]
    fn naive_date_from_string() {
        let date = NaiveDate::parse_from_str("2023-05-01", "%Y-%m-%d").ok();
        assert_eq!(date, NaiveDate::from_ymd_opt(2023, 5, 1));
    }

    #[test]
    fn test_num_years_by() {
        let employee = Employee::new(
            NaiveDate::from_ymd_opt(2020, 1, 1).unwrap(),
            Some(NaiveDate::from_ymd_opt(2020, 12, 31).unwrap()),
        );

        assert_relative_eq!(
            employee
                .num_years_by(NaiveDate::from_ymd_opt(2020, 1, 1).unwrap())
                .unwrap(),
            0.0,
            epsilon = 0.001
        );
        assert_relative_eq!(
            employee
                .num_years_by(NaiveDate::from_ymd_opt(2020, 12, 31).unwrap())
                .unwrap(),
            0.999,
            epsilon = 0.001
        );
        assert_relative_eq!(
            employee
                .num_years_by(NaiveDate::from_ymd_opt(2020, 6, 1).unwrap())
                .unwrap(),
            0.416,
            epsilon = 0.001
        );
        assert_eq!(
            employee.num_years_by(NaiveDate::from_ymd_opt(2021, 1, 1).unwrap()),
            None
        );
        assert_eq!(
            employee.num_years_by(NaiveDate::from_ymd_opt(2019, 1, 1).unwrap()),
            None
        );
    }
}
