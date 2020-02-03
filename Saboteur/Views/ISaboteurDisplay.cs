
namespace Saboteur.Views
{
    public delegate void IDSelectDelegate(int ID);
    public delegate void PositionSelectDelegate(int row, int column);
    public delegate void CardUsedDelegate();

    public interface ISaboteurDisplay
    {
        IDSelectDelegate SelectCard { get; set; }
        IDSelectDelegate TargetTo { get; set; }
        PositionSelectDelegate PlaceOn { get; set; }
        CardUsedDelegate RotateCard { get; set; }
        CardUsedDelegate Discard { get; set; }

        void Log(string LogMessage);
        void Show();
    }
}
