namespace Agazaty.Data.Enums
{
    public static class LeaveTypes
    {
        public enum Leaves
        {
            اعتيادية,
            عارضة,
            مرضية
        }

        public static readonly List<string> res = Enum.GetNames(typeof(Leaves)).ToList();
    }
}
