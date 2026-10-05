namespace UP01_Prokofev_Bank
{
    public partial class Entities
    {
        private static Entities _context;

        public static Entities GetContext()
        {
            if (_context == null)
                _context = new Entities();
            return _context;
        }
    }
}
