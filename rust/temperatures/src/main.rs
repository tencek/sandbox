mod date;
mod myasync;
mod temp;

use chrono::NaiveDate;
use date::DateRange;

#[derive(Debug)]
struct Stanice(String);

fn make_url(stanice: Stanice, date: NaiveDate) -> String {
    format!(
        "http://www.in-pocasi.cz/aktualni-pocasi/{}/?historie={}",
        stanice.0,
        date.format("%Y-%m-%d")
    )
}

use futures::executor::block_on;
use scraper::{Html, Selector};

async fn load_html(url: String) -> Result<Html, reqwest::Error> {
    let response_text = reqwest::get(&url).await?.text().await?;
    let document = Html::parse_document(&response_text);
    Ok(document)
}

use temp::DegC;

fn main() {
    let temp1 = DegC::from_str("2E34E-3°C");

    match temp1 {
        Ok(temp) => println!("Temp: {}", temp),
        Err(err) => eprintln!("Error: {}", err),
    }

    myasync::mymain();

    let stanice = Stanice("zlin_centrum".to_string());
    let date = NaiveDate::from_ymd(2020, 12, 1);
    let url = make_url(stanice, date);

    let runtime = tokio::runtime::Runtime::new().unwrap();
    match runtime.block_on(load_html(url)) {
        Ok(html) => {
            let first_table = Selector::parse("table").unwrap();
            let first_row = Selector::parse("tr").unwrap();
            let cell = Selector::parse("td").unwrap();

            if let Some(cells) = html
                .select(&first_table)
                .next()
                .and_then(|table| table.select(&first_row).next())
                .map(|row| row.select(&cell).collect::<Vec<_>>())
            {
                if cells.len() >= 2 {
                    let cas_mereni = cells[0].text().collect::<String>();
                    let teplota = cells[1].text().collect::<String>();
                    println!("Čas měření: {}, Teplota: {}", cas_mereni, teplota);
                }
            }
        }
        Err(err) => eprintln!("Error: {}", err),
    }
}
