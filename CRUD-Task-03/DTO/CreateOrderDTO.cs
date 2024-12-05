namespace CRUD.DTO
{
    public class CreateOrderDTO
    {
        public CreateOrderHeaderDTO header { get; set; }
        public List<CreateOrderRowDTO> rows { get; set; }
    }
}
