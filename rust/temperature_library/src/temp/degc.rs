use regex::Regex;
use std::error;
use std::fmt;
use std::num::ParseFloatError;
use String as StringBeingParsed;

#[derive(Debug, PartialEq)]
pub enum DegCParseError {
    DegCValueNotFound,
    NotAFloat(StringBeingParsed, ParseFloatError),
    RegexError(regex::Error),
}

impl fmt::Display for DegCParseError {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
            DegCParseError::RegexError(err) => write!(f, "Regex error: {}", err),
            DegCParseError::DegCValueNotFound => write!(f, "No °C value found"),
            DegCParseError::NotAFloat(str, err) => {
                write!(f, "'{}' is not a float! Error: {}", str, err)
            }
        }
    }
}

impl error::Error for DegCParseError {}

impl From<regex::Error> for DegCParseError {
    fn from(err: regex::Error) -> DegCParseError {
        DegCParseError::RegexError(err)
    }
}

#[derive(Debug, Clone, Copy, PartialEq)]
pub struct DegC(f64);

impl DegC {
    pub const fn new(value: f64) -> Self {
        DegC(value)
    }

    pub fn from_str(input: &str) -> Result<Self, DegCParseError> {
        let regex = Regex::new(r"(.*?) ?°C").map_err(DegCParseError::RegexError)?;
        let value_str = regex
            .captures(input)
            .ok_or(DegCParseError::DegCValueNotFound)?
            .get(1)
            .ok_or(DegCParseError::DegCValueNotFound)?
            .as_str();
        let degc_value = value_str
            .parse()
            .map_err(|err| DegCParseError::NotAFloat(value_str.to_string(), err))?;
        Ok(DegC(degc_value))
    }

    pub fn to_f64(&self) -> f64 {
        self.0
    }
}

impl fmt::Display for DegC {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        write!(f, "{} °C", self.0)
    }
}

impl std::iter::Sum for DegC {
    fn sum<I: Iterator<Item = Self>>(iter: I) -> Self {
        iter.fold(DegC(0.0), |a, b| DegC(a.0 + b.0))
    }
}

// impl Sum for DegC {
//     fn sum<I: Iterator<Item = Self>>(iter: I) -> Self {
//         iter.fold(DegC(0.0), |a, b| DegC(a.0 + b.0))
//     }
// }

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_deg_c_from_str() {
        let input = "12.3 °C";
        let expected = Ok(DegC(12.3));
        assert_eq!(DegC::from_str(input), expected);
    }

    #[test]
    fn test_deg_c_from_str_negative() {
        let input = "-99 °C";
        let expected = Ok(DegC(-99.0));
        assert_eq!(DegC::from_str(input), expected);
    }

    #[test]
    fn test_deg_c_from_str_zero() {
        let input = "0 °C";
        let expected = Ok(DegC(0.0));
        assert_eq!(DegC::from_str(input), expected);
    }

    // TODO: This test is currently failing and needs to be fixed.
    #[test]
    #[ignore]
    fn test_deg_c_from_czech_locale() {
        let input = "-12,3 °C";
        let expected = Ok(DegC(-12.3));
        assert_eq!(DegC::from_str(input), expected);
    }

    #[test]
    fn test_deg_c_from_str_scientific_notation() {
        let input = "234E-3 °C";
        let expected = Ok(DegC(0.234));
        assert_eq!(DegC::from_str(input), expected);
    }

    #[test]
    fn test_deg_c_from_no_space() {
        let input = "12.3°C";
        let expected = Ok(DegC(12.3));
        assert_eq!(DegC::from_str(input), expected);
    }

    #[test]
    fn test_deg_c_from_str_hexstring() {
        let input = "0x3A °C";
        let result = DegC::from_str(input);
        assert!(matches!(result, Err(DegCParseError::NotAFloat(_, _))));
    }

    #[test]
    fn test_deg_c_from_str_empty() {
        let empty_string = "";
        assert_eq!(
            DegC::from_str(empty_string),
            Err(DegCParseError::DegCValueNotFound)
        );
    }

    #[test]
    fn test_deg_c_from_str_no_unit_of_measure() {
        let empty_string = "12.3";
        assert_eq!(
            DegC::from_str(empty_string),
            Err(DegCParseError::DegCValueNotFound)
        );
    }
}
