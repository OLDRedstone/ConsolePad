using static ConsolePad.Extensions;

int a = 1;
List<string> names = ["John", "Mike", "Peter"];
Dictionary<string, int> ages = new()
{
	{"John", 23 },
	{"Mike", 25 },
	{"Peter", 22 },
};
Person John = new()
{
	Name = "John",
	Age = 23,
	Birth = new(2000, 2, 5),
	City = "City",
	Country = "Country",
	Phone = 87654321,
	Father = new()
	{
		Name = "Jake",
		Age = 49,
		Birth = new(1960,9,2),
		City = "City",
		Country = "Country",
		Phone = 87654322,
	}
};

a.Dump();
names.Dump();
ages.Dump();
John.Dump();


class Person
{
	public string Name { get; set; }
	public int Age {  get; set; }
	public DateTime Birth { get; set; }
	public string City { get; set; }
	public string Country { get; set; }
	public int Phone { get; set; }
	public Person Father { get; set; }
	public Person Mother { get; set; }
}
