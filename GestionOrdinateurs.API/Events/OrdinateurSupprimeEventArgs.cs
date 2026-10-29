namespace GestionOrdinateurs.API.Events;

public class OrdinateurSupprimeEventArgs : EventArgs
{
    public int OrdinateurId { get; }

    public OrdinateurSupprimeEventArgs(int ordinateurId)
    {
        OrdinateurId = ordinateurId;
    }
}