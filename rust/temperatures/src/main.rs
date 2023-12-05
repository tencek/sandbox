mod deg_c;
use deg_c::DegC;

use chrono::{DateTime, Local};
use chrono::NaiveDate;

#[derive(Debug)]
struct Stanice(String);

#[derive(Debug)]
struct DateRange {
    from_date: DateTime<Local>,
    to_date: DateTime<Local>,
}

fn make_url(stanice: Stanice, date: NaiveDate) -> String {
    format!("http://www.in-pocasi.cz/aktualni-pocasi/{}/?historie={}", stanice.0, date.format("%Y-%m-%d"))
}

use reqwest::Error;
use tokio::runtime::Runtime;
use scraper::{Html, Selector};

async fn load_html(url: String) -> Result<Html, Error> {
    let resp = reqwest::get(&url).await?;
    let body = resp.text().await?;
    let document = Html::parse_document(&body);
    Ok(document)
}

fn main() {
    let stanice = Stanice("zlin_centrum".to_string());
    let date = NaiveDate::from_ymd(2020, 12, 1);
    let url = make_url(stanice, date);

    let rt = Runtime::new().unwrap();
    match rt.block_on(load_html(url)) {
        Ok(html) => {
            let first_table = Selector::parse("table").unwrap();
            let first_row = Selector::parse("tr").unwrap();
            let cell = Selector::parse("td").unwrap();

            if let Some(cells) = html.select(&first_table).next()
                .and_then(|table| table.select(&first_row).next())
                .map(|row| row.select(&cell).collect::<Vec<_>>()) {
                if cells.len() >= 2 {
                    let cas_mereni = cells[0].text().collect::<String>();
                    let teplota = cells[1].text().collect::<String>();
                    println!("Čas měření: {}, Teplota: {}", cas_mereni, teplota);
                }
            }
        },
        Err(err) => eprintln!("Error: {}", err),
    }
}