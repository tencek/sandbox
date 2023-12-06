use temperature_library::temp::history::{
    date_range::DateRange, in_pocasi::InPocasi, loader::Loader,
};

fn main() {
    // Create a date range for history
    let date_range = DateRange {
        start: chrono::Utc::now() - chrono::Duration::days(1),
        end: chrono::Utc::now(),
    };
    println!("Date Range: {:?} to {:?}", date_range.start, date_range.end);

    // Use the InPocasi loader to load temperature data
    match InPocasi::new() {
        Ok(loader) => loader
            .get_available_locations()
            .iter()
            .for_each(|location| {
                let temperatures = loader.get_temperatures(&location, &date_range);
                let sum = temperatures.iter().map(|t| t.to_f64()).sum::<f64>();
                let count = temperatures.len() as f64;
                let avg = sum / count;
                println!(
                    "Location: {} avg: {:.3} ({} values)",
                    location.name, avg, count
                );
            }),
        Err(err) => {
            println!(
                "Could not create loader of type {}: {}",
                std::any::type_name::<InPocasi>(),
                err
            );
            return;
        }
    };
}
