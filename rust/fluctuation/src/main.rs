mod assets;
mod employee;
use employee::Employees;

fn main() {
    use chrono::Local;
    use itertools::Itertools;
    use std::iter::once;

    match Employees::try_from(assets::EMPLOYEES) {
        Ok(employees) => {
            let mut wtr = csv::Writer::from_path("active_employees_by_date.csv").unwrap();
            wtr.write_record(&["date", "active_count", "num_years_avg"])
                .unwrap();

            employees
                .iter()
                .flat_map(|e| {
                    if e.is_active() {
                        once(e.board_in)
                            .chain(once(e.board_in.pred_opt().unwrap()))
                            .chain(once(e.board_in)) // padding
                            .chain(once(e.board_in)) // padding
                    } else {
                        once(e.board_in)
                            .chain(once(e.board_in.pred_opt().unwrap()))
                            .chain(once(e.board_out.unwrap()))
                            .chain(once(e.board_out.unwrap().succ_opt().unwrap()))
                    }
                })
                .chain(once(Local::now().date_naive()))
                .unique()
                .sorted()
                .map(|date| {
                    let active_employees =
                        employees.iter().filter(|e| e.was_active_by(date.clone()));
                    let active_count = active_employees.clone().count();
                    let num_years_avg = active_employees
                        .map(|e| e.num_years_by(date.clone()).unwrap())
                        .sum::<f64>()
                        / active_count as f64;
                    (date, active_count, num_years_avg)
                })
                .for_each(|(date, active_count, num_years_avg)| {
                    wtr.write_record(&[
                        date.to_string(),
                        active_count.to_string(),
                        num_years_avg.to_string(),
                    ])
                    .unwrap();
                });

            wtr.flush().unwrap();

            let active_employees = employees.iter().filter(|e| e.is_active());
            let active_employee_count = active_employees.clone().count();
            let num_years_avg = active_employees
                .clone()
                .map(|e| e.num_years_by(Local::now().date_naive()).unwrap())
                .sum::<f64>()
                / active_employee_count as f64;
            println!("Active employees: {}", active_employee_count);
            println!("Average number of years: {:.3}", num_years_avg);
        }
        Err(e) => eprintln!("Error: {}", e),
    }
}
