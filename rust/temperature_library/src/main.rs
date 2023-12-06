use temperature_library::temp::{
    degc::DegC,
    history::{date_range::DateRange, in_pocasi::InPocasi, loader::Loader},
    location::Location,
};

fn main() {
    // Create a temperature value in degrees Celsius
    let temperature = DegC::new(23.5);
    println!("Temperature: {}°C", temperature.value);

    // Create a date range for history
    let date_range = DateRange {
        start: chrono::Utc::now(),
        end: chrono::Utc::now() + chrono::Duration::days(7),
    };
    println!("Date Range: {:?} to {:?}", date_range.start, date_range.end);

    // Create a location with a name and timezone
    let location = Location {
        name: String::from("Sample Location"),
        timezone: chrono_tz::Europe::Berlin,
    };
    println!("Location: {} ({})", location.name, location.timezone);

    // Use the InPocasi loader to load temperature data
    let in_pocasi_loader = InPocasi;
    let data = in_pocasi_loader.get_temperatures(&location, &date_range);
    println!("Loaded temperature data: {:?}", data);
}
