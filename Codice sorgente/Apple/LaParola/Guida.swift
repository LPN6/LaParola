//
//  Guida.swift
//  LaParola
//
//  Created by admin on 29/04/24.
//

import SwiftUI
import WebKit

struct Guida: View {
    
    @StateObject private var viewModel = WebViewModel()
    @State private var webView: WKWebView? = nil
    
    var body: some View {
#if os(iOS)
        WebView(url:costruisciHtml(), anchor:.constant(""), viewModel: viewModel, onSwipe: { _ in })
            .navigationTitle("Guida")
        #else
        WebView(url:costruisciHtml(), anchor:.constant(""), viewModel: viewModel)
            .navigationTitle("Guida")
        #endif
    }
    
    func costruisciHtml() -> String {
#if os(macOS)
        let fileGuida = "guida_it"
#endif
#if os(iOS)
        let fileGuida = "guida_ios"
#endif
        guard let filepath = Bundle.main.path(forResource: fileGuida, ofType: "txt")
        else {
            return String(localized:"Errore sconosciuto nella lettura del testo della Guida.")
        }
        do {
            var contents = try String(contentsOfFile: filepath)
            if contents.contains("<body><div>") {
                // questo codice anche in Testi.swift
                let fontNomeDaUsare = (ContentView.testi.formato.fontNome=="SF Pro" ? "-apple-system" : ContentView.testi.formato.fontNome) // lo spazio nel nome è un problema
                let nomeFontStringa = (!ContentView.testi.formato.fontNome.isEmpty ? "font-family:" + fontNomeDaUsare + ";" : "");
                let s = "<body><div style='" + nomeFontStringa + "font-size:" + String(ContentView.testi.formato.fontDimensione) + "px;';>"
                contents = contents.replacingOccurrences(of: "<body><div>", with: s)
            }
            return contents
        }
        catch {
            
        }
        return String(localized:"Errore sconosciuto nella lettura del testo della Guida.")
    }
}

#Preview {
    Guida()
}
