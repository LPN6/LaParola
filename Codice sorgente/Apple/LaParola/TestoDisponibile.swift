//
//  TestoDisponibile.swift
//  LaParola
//
//  Created by admin on 09/03/24.
//

import Foundation

struct TestoDisponibile
{
    var nome: String = ""
    var componente: String = ""
    var nomeFile: String = ""
    var tipo: String = ""
    var versioneAttuale: String = ""
    var versioneNuova: String = ""
    var url: [String] = []
    var dimensione: String = ""
    var lingua: String = ""
}

public enum AggiornamentoTipo
{
    case NonScaricato
    case DaAggiornare
    case Aggiornato
}

