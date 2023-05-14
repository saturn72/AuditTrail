namespace Server.Domain
{
    public record Product
    {
        public int Id { get; init; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public List<Category>? Categories { get; set; } = new();
    }
}
