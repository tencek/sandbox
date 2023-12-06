use chrono::{DateTime, Local};

#[derive(Debug)]
pub struct DateRange {
    from_date: DateTime<Local>,
    to_date: DateTime<Local>,
}
