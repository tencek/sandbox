use chrono::NaiveDate;
use scraper::{Html, Selector};
use tokio::runtime::Runtime;

use crate::temp::history::date_range::dates_in_range2;

use super::super::{degc::DegC, location::Location};
use super::{date_range::DateRange, loader::Loader};

pub struct InPocasi {
    runtime: Runtime,
}

pub enum InPocasiError {
    CouldNotCreateRuntime(std::io::Error),
}

impl std::fmt::Display for InPocasiError {
    fn fmt(&self, f: &mut std::fmt::Formatter) -> std::fmt::Result {
        match self {
            InPocasiError::CouldNotCreateRuntime(err) => {
                write!(
                    f,
                    "Could not create runtime of type {}: {}",
                    std::any::type_name::<Runtime>(),
                    err
                )
            }
        }
    }
}

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
            },
        ]
    }

    fn get_temperatures(&self, location: &Location, date_range: &DateRange) -> Vec<DegC> {
        let future_data = dates_in_range2(date_range)
            .iter()
            .map(|date| make_url(location, date))
            .map(|url| load_html_async(url))
            .collect::<Vec<_>>();
        let future_data = futures::future::join_all(future_data);
        let data = self.runtime.block_on(future_data);
        // let errors = data.iter().filter(|result| result.is_err());
        let only_data = data
            .into_iter()
            .filter(|result| result.is_ok())
            .map(|result| result.unwrap())
            .collect::<Vec<_>>();
        only_data
            .iter()
            .flat_map(read_temp_data)
            .collect::<Vec<_>>()
    }
}

impl InPocasi {
    pub fn new() -> Result<Self, InPocasiError> {
        let runtime =
            tokio::runtime::Runtime::new().map_err(InPocasiError::CouldNotCreateRuntime)?;
        Ok(InPocasi { runtime })
    }
}

fn read_temp_data(html: &Html) -> Vec<DegC> {
    let table = Selector::parse("table").unwrap(); // todo - error handling
    let tbody = Selector::parse("tbody").unwrap(); // todo - error handling
    let tr = Selector::parse("tr").unwrap(); // todo - error handling
    let td = Selector::parse("td").unwrap(); // todo - error handling

    html.select(&table)
        .next()
        .unwrap()
        .select(&tbody)
        .next()
        .unwrap()
        .select(&tr)
        .map(|row| {
            let temp_str = row.select(&td).nth(1).unwrap().inner_html();
            DegC::from_str(&temp_str).unwrap()
        })
        .collect::<Vec<_>>()
}

async fn load_html_async(url: String) -> Result<Html, reqwest::Error> {
    let response_text = reqwest::get(url).await?.text().await?;
    let document = Html::parse_document(&response_text);
    Ok(document)
}

fn make_url(location: &Location, date: &NaiveDate) -> String {
    format!(
        "http://www.in-pocasi.cz/aktualni-pocasi/{}/?historie={}",
        location.name,
        date.format("%Y-%m-%d")
    )
}
