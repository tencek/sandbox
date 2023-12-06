use super::date_range::DateRange;
use super::super::location::Location;
use super::super::degc::DegC;

pub trait Loader {
    fn get_available_locations(&self) -> Vec<Location>;
    fn get_temperatures(&self, location: &Location, date_range: &DateRange) -> Vec<DegC>;
}
