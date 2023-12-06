
#[derive(Debug)]
pub struct DegC {
    pub value: f64,
}

impl DegC {
    pub fn new(value: f64) -> Self {
        DegC { value }
    }
}
