﻿namespace CRUD_Task_03.DTO
{
    public class GetOrderDetailsTest
    {
        public long OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime? OrderDate { get; set; }
        public List<GetOrderDetailsRowDTO> Rows { get; set; }
    }
}
