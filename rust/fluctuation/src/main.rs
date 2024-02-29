mod employee;

use anyhow::anyhow;
use anyhow::Error;
use anyhow::Result;
use chrono::NaiveDate;
use employee::Employee;

fn load_employees() -> Result<Vec<Employee>> {
    let data = include_str!("../assets/employees.csv");
    let mut rdr = csv::Reader::from_reader(data.as_bytes());

    rdr.records()
        .map(|record| {
            let record = record.expect("a CSV record");
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
            Ok::<Employee, Error>(Employee::new(board_in, board_out))
        })
        .collect()
}

fn main() {
    match load_employees() {
        Ok(employees) => {
            let active: Vec<&Employee> = employees
                .iter()
                .filter(|employee| employee.is_active())
                .collect();
            println!("Active employees: {:?}", active.len());
        }
        Err(e) => eprintln!("Error: {}", e),
    }
}
