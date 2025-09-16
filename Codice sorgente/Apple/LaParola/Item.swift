//
//  Item.swift
//  LaParola
//
//  Created by admin on 08/01/24.
//

import Foundation
import SwiftData
import SwiftUI

@Model
final class Item {
    //var itemancora: String = ""
    var versione: String = ""
    var html: String = ""
    var htmlGenerated = false
    var htmlTimestamp = Date()
    var espressione = ""
    var libroScelto = 0 // a necessary hack to work around a problem with nested popovers, in the popover to choose a chapter / verse
    var capitoloScelto = 0
    var order = -1
    
    init(versione: String) {
        self.versione = versione
    }
    
   /* func updateAncora(_ nuovoAncora: String) {
     //   itemancora = nuovoAncora
    }*/
    
    func generaTestoInThread(completion: @escaping (String) -> Void) {
        var t = ""
        DispatchQueue.global(qos: .default).async {
            t = ContentView.testi.testoRicerca(self.espressione, self.versione)
            DispatchQueue.main.async {
                completion(t)
            }
        }
    }
    
    public func generaTesto(_ testoUguale: Bool = false) {
        htmlGenerated = false;
        if !testoUguale {
            generaTestoInThread { t in
                DispatchQueue.main.async {
                    self.html = ContentView.testi.mostraHtml(t)
                    self.htmlTimestamp = Date()
                    self.htmlGenerated = true
                }
            }
        } else {
            let temp = "Q" + self.html
            self.html = String(temp.dropFirst())
            self.htmlTimestamp = Date()
            self.htmlGenerated = true
        }
    }
       
    public func spostaTesto(_ libro:Int, _ capitolo:Int) {
        // chiamata da ContentView (per i preferiti)
        // bisogna imposta viewModel.wvmancora prima di chiamare
        /*var ancoraTemp = (libro <= 9 ? "0" + String(libro) : String(libro));
        let capitoloStringa = "00" + String(capitolo);
        ancoraTemp += capitoloStringa[(capitoloStringa.count - 3)...];
        let versettoStringa = "00" + String(versetto);
        ancoraTemp += versettoStringa[(versettoStringa.count - 3)...];
        //itemancora = ancoraTemp*/
        aggiornaTesto(libro, capitolo)
    }
    
    public func aggiornaTesto(_ libro:Int, _ capitolo:Int) {
        var riferimento = ContentView.testi.normalizzaRiferimento(Riferimento(libro, capitolo), RiferimentoFormato.AbbreviazioneRiconosciuta)
        if !riferimento[riferimento.count-1].isNumber() {
            riferimento += " 1-255" // occorre con i libri con un capitolo
        }
        espressione = riferimento
        generaTesto()
    }
}
