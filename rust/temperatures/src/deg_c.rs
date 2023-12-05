use regex::Regex;

#[derive(Debug, Clone, Copy, PartialEq)]
pub struct DegC(f64);

impl DegC {
    pub const fn new(value: f64) -> Self {
        DegC(value)
    }

    pub fn from_str(input: &str) -> Option<Self> {
        let re = Regex::new(r"(.*)°C").ok()?;
        let value_str = re.captures(input)?.get(1)?.as_str();
        let value = value_str.parse().ok()?;
        Some(DegC(value))
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_deg_c_from_str() {
        let input = "12.3°C";
        let expected = Some(DegC(12.3));
        assert_eq!(DegC::from_str(input), expected);
    }

    #[test]
    fn test_deg_c_from_str_invalid() {
        let input = "invalid";
        assert_eq!(DegC::from_str(input), None);
    }
}