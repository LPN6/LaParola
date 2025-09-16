//
//  OccorrenzaParola.swift
//  LaParola
//
//  Created by admin on 15/02/24.
//

import Foundation

/// <remarks>
/// Per la concordanza, dà il numero del versetto o della nota e il numero della parola nella voce
/// </remarks>
public struct OccorrenzaParola : Comparable
{
    
    var voce:UInt
    var parola:UInt16
    
    init(_ voce: UInt = 0, _ parola: UInt16 = 0) {
        self.voce = voce
        self.parola = parola
    }
        
    /// <summary>
    /// Confronta un altro oggetto di tipo OccorrenzaParola con quello attuale.
    /// </summary>
    /// <param name="obj">L'altro oggetto di tipo OccorrenzaParola da confrontare.</param>
    /// <returns>-1 se questa parola è prima dell'altro, 0 se è uguale, 1 se è dopo.</returns>
    public func compareTo(_ op:OccorrenzaParola) -> Int
    {
        if (self.voce < op.voce) {
            return -1;
        }
        else if (self.voce > op.voce) {
            return 1;
        }
        else
        {
            if (self.parola < op.parola) {
                return -1;
            }
            else if (self.parola > op.parola) {
                return 1;
            }
            else {
                return 0;
            }
        }
    }
    
    /// <summary>
    /// Restituisce se due oggetti sono uguali.
    /// </summary>
    /// <param name="obj">Un oggetto di tipo OccorrenzaParola a cui paragonare questo oggetto.</param>
    /// <returns>True se le occorrenze sono uguali.</returns>
    public func equals(_ obj:OccorrenzaParola) -> Bool
    {
        return (compareTo(obj) == 0);
    }
    
    /*
    /// <summary>
    /// Calcola il hash code.
    /// </summary>
    /// <returns>Il hash code.</returns>
    public func GetHashCode() -> Int
    {
        return (int)(voce / 2) ^ parola;
    }
    */
    
     
    public static func < (lhs: OccorrenzaParola, rhs: OccorrenzaParola) -> Bool {
        return lhs.compareTo(rhs) < 0;
    }
    
    public static func == (lhs: OccorrenzaParola, rhs: OccorrenzaParola) -> Bool {
        return lhs.compareTo(rhs) == 0;
    }
    /*
     
     /// <summary>
     /// Se due oggetti sono uguali.
     /// </summary>
     /// <param name="primoOggetto">Primo oggetto.</param>
     /// <param name="secondoOggetto">Secondo oggetto.</param>
     /// <returns>True se sono uguali.</returns>
     public static bool operator ==(OccorrenzaParola primoOggetto, OccorrenzaParola secondoOggetto)
     {
     if (Object.ReferenceEquals(primoOggetto, null))
     return Object.ReferenceEquals(secondoOggetto, null);
     return primoOggetto.Equals(secondoOggetto);
     }
     
     /// <summary>
     /// Se due oggetti sono diversi.
     /// </summary>
     /// <param name="primoOggetto">Primo oggetto.</param>
     /// <param name="secondoOggetto">Secondo oggetto.</param>
     /// <returns>True se sono diversi.</returns>
     public static bool operator !=(OccorrenzaParola primoOggetto, OccorrenzaParola secondoOggetto)
     {
     return !(primoOggetto == secondoOggetto);
     }
     
     
     /// <summary>
     /// Se una parola appare dopo un'altra.
     /// </summary>
     /// <param name="primoOggetto">Prima parola.</param>
     /// <param name="secondoOggetto">Seconda parola.</param>
     /// <returns>True se la prima appare dopo la seconda.</returns>
     public static bool operator >(OccorrenzaParola primoOggetto, OccorrenzaParola secondoOggetto)
     {
     return primoOggetto.CompareTo(secondoOggetto) > 0;
     }
     */
}
