using System.Windows.Documents;

namespace LaParola.DocumentViews;

public interface IFlowDocumentHost
{
    void SetDocument(FlowDocument doc);

    /// <summary>
    /// Esegue <paramref name="azione"/> (tipicamente una colorazione del FlowDocument, es. per
    /// l'evidenziazione karaoke del lettore vocale) senza che l'host la consideri una modifica
    /// dell'utente - negli editor editabili non deve sporcare lo stato "non salvato".
    /// </summary>
    void EseguiSenzaSporcareDocumento(Action azione);
}
