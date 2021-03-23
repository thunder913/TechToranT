namespace RestaurantMenuProject.Data.Models.Enums
{
    public enum ProcessType
    {
        Pending = 0, // The client just send a new order
        InProcess = 1, // The order is approved by the waiter
        Cooking = 2, // The cook accepted the order and started
        Cooked = 3, // The cooking is done
        Delivered = 4, // The waiter delivered everything
        Completed = 5, // Everything is delivered and paid for
        Cancelled = 6, // The client/cook/waiter cancelled the order and it should not be cooked
    }
}
