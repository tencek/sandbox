mod assets;
mod employee;

use employee::Employee;
use employee::Employees;

fn main() {
    use itertools::Itertools;
    use std::iter::once;

    match Employees::try_from(assets::EMPLOYEES) {
        Ok(employees) => {
            let active: Vec<&Employee> = employees
                .iter()
                .filter(|employee| employee.is_active())
                .collect();
            println!("Active employees: {:?}", active.len());
            let special_dates_sorted = employees
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
                .unique()
                .sorted()
                .collect::<Vec<_>>();
            println!("Distinct dates: {:?}", special_dates_sorted.len());
        }
        Err(e) => eprintln!("Error: {}", e),
    }
}
