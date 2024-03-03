mod csv_file;
pub use csv_file::CsvFile;

pub const EMPLOYEES: CsvFile = CsvFile {
    data: include_str!("../assets/employees.csv"),
};
