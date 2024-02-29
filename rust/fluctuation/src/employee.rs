use chrono::NaiveDate;

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

    pub fn was_active_at(&self, date: NaiveDate) -> bool {
        if let Some(board_out) = self.board_out {
            date >= self.board_in && date <= board_out
        } else {
            date >= self.board_in
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use chrono::NaiveDate;
    use Result;

    #[test]
    fn test_was_active_at() {
        let employee = Employee::new(
            NaiveDate::from_ymd_opt(2020, 1, 1).unwrap(),
            Some(NaiveDate::from_ymd_opt(2020, 12, 31).unwrap()),
        );

        assert!(employee.was_active_at(NaiveDate::from_ymd_opt(2020, 1, 1).unwrap()));
        assert!(employee.was_active_at(NaiveDate::from_ymd_opt(2020, 12, 31).unwrap()));
        assert!(employee.was_active_at(NaiveDate::from_ymd_opt(2020, 6, 1).unwrap()));
        assert!(!employee.was_active_at(NaiveDate::from_ymd_opt(2021, 1, 1).unwrap()));
    }

    #[test]
    fn naive_date_from_string() {
        let date = NaiveDate::parse_from_str("2023-05-01", "%Y-%m-%d").ok();
        assert_eq!(date, NaiveDate::from_ymd_opt(2023, 5, 1));
    }
}
