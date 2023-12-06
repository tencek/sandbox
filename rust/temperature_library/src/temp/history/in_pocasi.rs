use chrono::NaiveDate;

use crate::temp::history::date_range::dates_in_range2;

use super::{date_range::DateRange, loader::Loader};
use super::super::{degc::DegC, location::Location};

pub struct InPocasi;

impl Loader for InPocasi {
    fn get_available_locations(&self) -> Vec<Location> {
        vec![
            Location {
                name: String::from("zlin"),
                timezone: chrono_tz::Europe::Prague,
            },
            Location {
                name: String::from("zlin_centrum"),
                timezone: chrono_tz::Europe::Prague,
            }
        ]
    }

    fn get_temperatures(&self, location: &Location, date_range: &DateRange) -> Vec<DegC> {
        let dates = dates_in_range2(date_range);
        let urls:Vec<String> = dates.iter().map(|date| make_url(location, date)).collect();
        vec![] // Placeholder return value
    }
}

fn make_url(location: &Location, date: &NaiveDate) -> String {
    format!("http://www.in-pocasi.cz/aktualni-pocasi/{}/?historie={}", location.name, date.format("%Y-%m-%d"))
}
