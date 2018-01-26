using Testownik.Models;

namespace Testownik.Model {
    public interface IAnswer {
        IContent Content { get; set; }
        int Key { get; set; }
    }
}