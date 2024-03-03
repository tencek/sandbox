use super::Employee;
use crate::assets::CsvFile;
use std::convert::TryFrom;
pub type Employees = Vec<Employee>;

impl TryFrom<CsvFile> for Employees {
    type Error = anyhow::Error;

    fn try_from(csv_file: CsvFile) -> Result<Self, Self::Error> {
        let mut rdr = csv::Reader::from_reader(csv_file.data.as_bytes());
        rdr.records()
            .map(|record| {
                let record = record?;
                Employee::try_from(record)
            })
            .collect::<Result<Vec<_>, _>>()
    }
}
