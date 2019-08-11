
public interface IStack
{
    int stackSize { get; set; }
    void SplitStack(int num);
    void Stack(IStack other);
    void UpdateStack();
}