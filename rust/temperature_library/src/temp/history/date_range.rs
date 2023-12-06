use std::ops::RangeInclusive;

use chrono::{Utc, DateTime, naive::NaiveDateDaysIterator, NaiveDate};

pub struct DateRange {
    pub start: DateTime<Utc>,
    pub end: DateTime<Utc>,
}

pub fn dates_in_range(date_range: &DateRange) -> RangeInclusive<NaiveDateDaysIterator> {
    let start_date = date_range.start.date_naive().iter_days();
    let end_date = date_range.start.date_naive().iter_days();
    start_date..=end_date
}

pub fn dates_in_range2(date_range: &DateRange) -> Vec<NaiveDate> {
    let start_date = date_range.start.date_naive();
    let end_date = date_range.end.date_naive();
    let mut dates = Vec::new();

    for date in start_date.iter_days().take_while(|&d| d <= end_date) {
        dates.push(date);
    }

    dates
}


