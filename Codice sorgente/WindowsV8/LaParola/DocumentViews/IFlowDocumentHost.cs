using System.Windows.Documents;

namespace LaParola.DocumentViews;

public interface IFlowDocumentHost
{
    void SetDocument(FlowDocument doc);
}
