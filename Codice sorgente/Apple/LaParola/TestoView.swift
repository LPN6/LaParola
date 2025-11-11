//
//  TestoView.swift
//  LaParola
//
//  Created by admin on 06/06/24.
//

import SwiftUI
import WebKit
import AVFoundation

public enum StatoTTS : Encodable, Decodable
{
    case Fermato
    case Inizia
    case Riprendi
    case InCorso
    case Ferma
    case Pausa
    case InPausa
};

class WebViewModel: ObservableObject {
    @Published var htmlTimestamp: Date = Date()
    @Published var branoClicked = false
    @Published var branoNuovo = false
    @Published var notaClicked = false
    @Published var clickedURL: String? = nil
    @Published var voce = StatoTTS.Fermato
    @Published var voceContinua = false
    @Published var prossimoCapitolo = ""
    @Published var lingua = ""
    @Published var wvmancora: String = ""
    @Published var ultimaAncora: String = ""
    @Published var webView: WKWebView?
    let synthesizer = AVSpeechSynthesizer()
    weak var coordinatorRef: Coordinator? = nil
    
    init() {
            self.webView = WKWebView()
    }
}

struct TestoView: View {
    @State private var searchText = ""
    @State private var searchTextSubmitted = ""
    @State private var menuLibro = false
    @State private var menuCapitolo = false
    @State private var menuVersetto = false
    @State private var pickerLibro = 1
    @State private var pickerCapitolo = 1
    @State private var pickerVersetto = 1
    @State private var pickerLibroOld = 0
    @State private var pickerCapitoloOld = 0
    @State private var pickerVersettoOld = 0
    //@State private var tvancora = ""
    @State private var spostaTestoConPicker = true
    @State private var dragStartTime: Date?
    @ObservedObject var viewModel: WebViewModel
    @Binding var selection:Item?
    var item: Item
    @Binding var tipoSceltaRiferimento: Int
    @Binding var pulsanteSintesiVocale: Bool
    @Binding var pulsantePreferiti: Bool
    @Binding var sintesiVocaleAutomatico: Bool
    @StateObject private var tvAlertViewModel = AlertViewModel()
    @Environment(\.colorScheme) var colorScheme
#if os(iOS)
    @Environment(\.horizontalSizeClass) var horizontalSizeClass
#endif
    
    let versioneDaUsare: String
#if os(macOS)
    let larghezzaCasellaLibri = 40.0
#endif
#if os(iOS)
    let larghezzaCasellaLibri = 50.0
#endif
    
    init(_ viewModel: WebViewModel, _ itemIn:Item, _ selezione: String, selection: Binding<Item?>, tipoSceltaRiferimento:Binding<Int>, pulsantePreferiti:Binding<Bool>, pulsanteSintesiVocale:Binding<Bool>, sintesiVocaleAutomatico:Binding<Bool>) {
        self.viewModel = viewModel
        self.item = itemIn
        self._selection = selection
        self._tipoSceltaRiferimento = tipoSceltaRiferimento
        self._pulsantePreferiti = pulsantePreferiti
        self._pulsanteSintesiVocale = pulsanteSintesiVocale
        self._sintesiVocaleAutomatico = sintesiVocaleAutomatico
        versioneDaUsare = (ContentView.testi.info(itemIn.versione).tipo == TestoTipi.Bibbia) ? itemIn.versione : (UserDefaults.standard.string(forKey: "versionepreferita") ?? "")
        if item.espressione.isEmpty {
            var primaLibro = 1
            if ContentView.testi.info(item.versione).tipo == TestoTipi.Bibbia {
                for i in stride(from:1, to:74, by:1) {
                    if ContentView.testi.capitoliInLibro(i, item.versione) > 0 {
                        primaLibro = i
                        break
                    }
                }
            }
            var r = ContentView.testi.normalizzaRiferimento(Riferimento(UInt8(primaLibro), 1, 1), RiferimentoFormato.AbbreviazioneRiconosciuta)
            if r.indexOf(":")>0 {
                r = r[0..<r.indexOf(":")]
            }
            if r.indexOf(",")>0 {
                r = r[0..<r.indexOf(",")]
            }
            item.espressione = r
            ricarica(false, true)
        }
    }
    
    func ricarica(_ testoUguale:Bool = false, _ inInit:Bool = false) {
        if (!testoUguale && !inInit) {
            viewModel.voce = StatoTTS.Ferma
        }
        item.generaTesto(testoUguale)
    }
    
    var body: some View {
        GeometryReader { geometry in
            //ZStack {
            contentWebView
                .id(item.htmlTimestamp)
            /*
            .onAppear(perform: {
                    if !viewModel.ultimaAncora.isEmpty {
                        spostaTestoAllaAncora()
                    }
                })*/
            .alert("", isPresented: $tvAlertViewModel.showAlert)
            {
                Button("OK") {
                    viewModel.wvmancora = viewModel.ultimaAncora
                }
            } message: {
                Text(tvAlertViewModel.alertMessage)
            }
            .gesture(MagnifyGesture(minimumScaleDelta: 0.01)
                     // .onChanged { value in
                     //     print(value.magnification)
                     //}
                .onEnded { value in
                    var newDim = ContentView.testi.formato.fontDimensione * value.magnification
                    if newDim > ContentView.maxDimensioneFont { newDim = ContentView.maxDimensioneFont }
                    if newDim < ContentView.minDimensioneFont { newDim = ContentView.minDimensioneFont }
                    ContentView.testi.formato.fontDimensione = newDim.rounded()
                    ricarica()
                }
            )
#if os(macOS)
            .searchable(text: $searchText, placement: .toolbar, prompt: "riferimento o parole")
#else
            .searchable(text: $searchText, placement: (horizontalSizeClass == .compact ?.navigationBarDrawer(displayMode: .always):.toolbar), prompt: "riferimento o parole")
#endif
            .searchPresentationToolbarBehavior(.automatic)
            .onSubmit(of: .search) {
                searchTextSubmitted = searchText
                item.espressione = (searchTextSubmitted.isEmpty ? " " : searchTextSubmitted)
                ricarica()
            }
            //.popover(isPresented: $viewModel.branoClicked) {
            // attachmentAnchor: .rect(.bounds),
#if os(macOS)
            .popover(isPresented: $viewModel.branoClicked) {
                if let riferimento = viewModel.clickedURL {
                    WebView(url:riferimentoURLATesto(riferimento), anchor:.constant(""), viewModel: viewModel)
                        .frame(width: geometry.size.width / 2)
                }
            }
#endif
#if os(iOS)
            .popover(isPresented: $viewModel.branoClicked,
                     attachmentAnchor:(horizontalSizeClass == .compact ? .rect(.bounds) : .point(UnitPoint(x:0.5, y:0.75))),
                     arrowEdge:(horizontalSizeClass == .compact ? .top : .bottom)) {
                if let riferimento = viewModel.clickedURL {
                    if horizontalSizeClass == .compact { // iPhone
                        WebView(url:riferimentoURLATesto(riferimento), anchor:.constant(""), viewModel: viewModel, onSwipe: { _ in } )
                    }
                    else { // iPad
                        WebView(url:riferimentoURLATesto(riferimento), anchor:.constant(""), viewModel: viewModel, onSwipe: { _ in } )
                            .frame(width: geometry.size.width / 2, height: geometry.size.height / 2)
                    }
                }
            }
#endif
                     .onChange(of: item.htmlGenerated) { _, generated in
                         //print("generated = \(generated)")
                         if generated {
                             spostaTestoAllaAncora()
                         }
                     }
                     .onChange(of: viewModel.branoClicked) { oldValue, newValue in
                         if !newValue {
                             selection = item
                         }
                     }
                     .onChange(of: viewModel.notaClicked) { oldValue, newValue in
                         // url = lpnn://Gen%2B1%3A20%2D25?ip=1
                         if newValue {
                             if let riferimento = viewModel.clickedURL {
                                 item.html = riferimentoURLATesto(riferimento)
                                 item.htmlTimestamp = Date()
                             }
                             viewModel.notaClicked = false
                         }
                     }
                     .onChange(of: viewModel.branoNuovo) { oldValue, newValue in
                         if newValue {
                             if let riferimento = viewModel.clickedURL {
                                 item.html = riferimentoURLATesto(riferimento, false)
                                 item.htmlTimestamp = Date()
                             }
                             viewModel.branoNuovo = false
                         }
                     }
                     .toolbar {
                         if tipoSceltaRiferimento != 1 {
                             ToolbarItem() {
                                 Button(action: {
                                     if menuCapitolo  {
                                         menuCapitolo = false
                                     }
                                     else {
                                         self.menuLibro.toggle()
                                     }
                                 }) {
                                     Label("", systemImage: "book.pages")
                                         .labelStyle(.iconOnly)
                                 }
                                 .popover(isPresented: $menuLibro) {
                                     ScrollView() {
                                         VStack {
                                             LazyVGrid(columns: Array(repeating: .init(.fixed(larghezzaCasellaLibri)), count: 5)) {
                                                 ForEach (1..<74, id:\.self) { nLibro in
                                                     if ContentView.testi.capitoliInLibro(nLibro, versioneDaUsare) > 0 {
                                                         Button(ContentView.testi.formato.libriAbbreviazioniUsate[nLibro]) {
                                                             item.libroScelto = nLibro
                                                             menuLibro = false;
                                                             self.menuCapitolo = true
                                                         }
                                                         .buttonStyle(PlainButtonStyle())
                                                         .frame(width:larghezzaCasellaLibri+2)
#if os(macOS)
                                                         .frame(height:29)
#endif
#if os(iOS)
                                                         .frame(height:44)
#endif
                                                         .background(coloreLibro(nLibro))
                                                     }
                                                 }
                                             }
                                             .padding()
                                         }
                                         .frame(width:6*larghezzaCasellaLibri+20)
                                     }
                                 }
                                 
                                 .popover(isPresented: $menuCapitolo) {
                                     ScrollView() {
                                         VStack {
                                             LazyVGrid(columns: Array(repeating: .init(.fixed(larghezzaCasellaLibri)), count: 5)) {
                                                 let nCapitoli = ContentView.testi.capitoliInLibro(item.libroScelto, versioneDaUsare)
                                                 ForEach (0..<nCapitoli, id:\.self) { nCapitolo in
                                                     Button(String(nCapitolo+1)) {
                                                         item.capitoloScelto = Int(nCapitolo)+1
                                                         menuCapitolo = false
                                                         menuVersetto = true;
                                                     }
                                                     .buttonStyle(PlainButtonStyle())
                                                     .frame(width:larghezzaCasellaLibri+2)
#if os(iOS)
                                                     .frame(height:44)
#endif
                                                 }
                                             }
                                             .padding()
                                         }
                                         .frame(width:6*larghezzaCasellaLibri+20)
                                     }
                                 }
                                 
                                 .popover(isPresented: $menuVersetto) {
                                     ScrollView() {
                                         VStack {
                                             LazyVGrid(columns: Array(repeating: .init(.fixed(larghezzaCasellaLibri)), count: 5)) {
                                                 let nVersetti = Int(ContentView.testi.versettiInCapitolo(item.libroScelto,item.capitoloScelto, versioneDaUsare))
                                                 ForEach (0..<nVersetti, id:\.self) { nVersetto in
                                                     Button(String(nVersetto+1)) {
                                                         menuVersetto = false
                                                         spostaTesto(item.libroScelto, item.capitoloScelto, nVersetto+1)
                                                     }
                                                     .buttonStyle(PlainButtonStyle())
                                                     .frame(width:larghezzaCasellaLibri+2)
#if os(iOS)
                                                     .frame(height:44)
#endif
                                                 }
                                             }
                                             .padding()
                                         }
                                         .frame(width:6*larghezzaCasellaLibri+20)
                                     }
                                 }
                             }
                         }
                         if tipoSceltaRiferimento != 0 {
                             ToolbarItem() {
                                 Picker("", selection: $pickerLibro) {
                                     ForEach(1..<74, id: \.self) { nLibro in
                                         // TOD2 per commentari, se versetto non esiste, va all'inizio del capitolo, forse meglio versetto precedente (basterebbe creare tutti gli anchor dei versetti mancanti e aggiungerli in Versione.swift alla riga testoDelBrano.append("<a id=\""+titoloNota[1..<9]+"\"></a>")
                                         // TOD2 cambiare qui se testo spostato in altri modi; in ricerca cosa visualizzare?
                                         if ContentView.testi.capitoliInLibro(nLibro, versioneDaUsare) > 0 {
#if os(iOS)
                                             Text(horizontalSizeClass == .compact ? ContentView.testi.formato.libriAbbreviazioniUsate[nLibro] : ContentView.testi.formato.libriNomi[nLibro]).tag(nLibro)
                                             #else
                                             Text(ContentView.testi.formato.libriNomi[nLibro]).tag(nLibro)
                                             #endif
                                         }
                                     }
                                 }
                                 .pickerStyle(MenuPickerStyle())
                                 .onChange(of: versioneDaUsare, initial: false) {
                                     if ContentView.testi.capitoliInLibro(pickerLibro, versioneDaUsare) == 0 {
                                         for nLibro in 1..<74 {
                                             if ContentView.testi.capitoliInLibro(nLibro, versioneDaUsare) > 0 {
                                                 spostaTestoConPicker = false
                                                 pickerLibro = nLibro
                                                 break
                                             }
                                         }
                                     }
                                 }
                                 .onAppear(perform: {
                                     if ContentView.testi.capitoliInLibro(pickerLibro, versioneDaUsare) == 0 {
                                         for nLibro in 1..<74 {
                                             if ContentView.testi.capitoliInLibro(nLibro, versioneDaUsare) > 0 {
                                                 pickerLibro = nLibro
                                                 break
                                             }
                                         }
                                     }
                                 })
                                 .onChange(of: pickerLibro) {
                                     if spostaTestoConPicker {
                                         pickerCapitolo = 1
                                         aggiornaTesto(pickerLibro, pickerCapitolo)
                                     }
                                     spostaTestoConPicker = true
                                 }
                             }
                             ToolbarItem() {
                                 Picker("", selection: $pickerCapitolo) {
                                     ForEach(1..<Int(ContentView.testi.capitoliInLibro(pickerLibro, versioneDaUsare))+1, id: \.self) { nCapitolo in
                                         Text(String(nCapitolo)).tag(nCapitolo)
                                     }
                                 }
                                 .pickerStyle(MenuPickerStyle())
                                 .onChange(of: pickerCapitolo) {
                                     //if pickerVersetto > ContentView.testi.versettiInCapitolo(pickerLibro, pickerCapitolo, versioneDaUsare) {
                                     pickerVersetto = 1
                                     aggiornaTesto(pickerLibro, pickerCapitolo)
                                     //}
                                 }
                             }
                             ToolbarItem() {
                                 Picker("", selection: $pickerVersetto) {
                                     ForEach(1..<Int(ContentView.testi.versettiInCapitolo(pickerLibro, pickerCapitolo, versioneDaUsare))+1, id: \.self) { nVersetto in
                                         Text(String(nVersetto)).tag(nVersetto)
                                     }
                                 }
                                 .pickerStyle(MenuPickerStyle())
                                 .onChange(of: pickerVersetto) {
                                     if (pickerLibro != pickerLibroOld || pickerCapitolo != pickerCapitoloOld || pickerVersetto != pickerVersettoOld) {
                                         spostaTesto(pickerLibro, pickerCapitolo, pickerVersetto)
                                         pickerLibroOld = pickerLibro
                                         pickerCapitoloOld = pickerCapitolo
                                         pickerVersettoOld = pickerVersetto
                                     }
                                 }
                             }
                         }
                         ToolbarItem() {
                             if pulsanteSintesiVocale {
                                 Button(action: {
                                     leggiTesto()
                                 }) {
                                     Label("", systemImage: (viewModel.voce==StatoTTS.Fermato || viewModel.voce==StatoTTS.InPausa) ? "speaker.wave.2" : "pause.fill")
                                         .labelStyle(.iconOnly)
                                 }
                             }
                         }
                         ToolbarItem() {
                             if pulsantePreferiti {
                                 Button(action: {
                                     aggiungiPreferito()
                                 }) {
                                     Label("", systemImage: "star")
                                         .labelStyle(.iconOnly)
                                 }
                             }
                         }
                     }
            //      }
            /*
             if progressVisibile {
             ProgressView()
             .scaleEffect(2)
             #if os(iOS)
             .tint(.blue)
             #endif
             }
             */ // TOD2 could not get it to work, because could not make progressVisibile usable in Item.swift
        }
    }

    @ViewBuilder
    private var contentWebView: some View {
#if os(iOS)
        WebView(url: item.html, anchor: $viewModel.wvmancora, viewModel: viewModel, onSwipe: handleSwipe)
#else
        WebView(url: item.html, anchor: $viewModel.wvmancora, viewModel: viewModel)
#endif
    }

private func handleSwipe(_ horizontalAmount:CGFloat) {
    let s = item.html
    let p = s.indexOf("<a id=")
    if p > -1 {
        let rif8=s[(p+7)..<(p+15)]
        var libro:Int = Int(rif8[0..<2]) ?? 0
        var cap:Int = Int(rif8[2..<5]) ?? 0
        if libro > 0 && cap > 0 {
            if horizontalAmount > 0 {
                cap -= 1
                if cap == 0 {
                    libro -= 1
                    while libro > 0 && ContentView.testi.capitoliInLibro(libro, versioneDaUsare) == 0 {
                        libro -= 1
                    }
                    if libro == 0 {
                        libro = 1
                        cap = 1
                    }
                    else {
                        cap = Int(ContentView.testi.capitoliInLibro(libro, versioneDaUsare))
                    }
                }
            }
            else {
                cap += 1
                if cap > ContentView.testi.capitoliInLibro(libro, versioneDaUsare) {
                    libro += 1
                    while libro < 74 && ContentView.testi.capitoliInLibro(libro, versioneDaUsare) == 0 {
                        libro += 1
                    }
                    if libro == 74 {
                        libro = 73
                        cap = Int(ContentView.testi.capitoliInLibro(libro, versioneDaUsare))
                    }
                    else {
                        cap = 1
                    }
                }
            }
            aggiornaTesto(libro, cap)
        }
    }
}

    private func aggiungiPreferito() {
        let script = "findFirstVisibleTarget();"
        var risultato = ""
#if os(macOS)
            if let webView = viewModel.coordinatorRef?.wkWebViewParent {
                        webView.evaluateJavaScript(script) { result, error in
                            if result != nil {
                                risultato = processaPreferito(result as! String)
                                tvAlertViewModel.showMessage(risultato.isEmpty ? "Non è stato possibile aggiungere il versetto ai preferiti.": "\(risultato) è stato aggiunto ai preferiti.")
                            }
                        }
                    }
#endif
#if os(iOS)
            viewModel.webView?.evaluateJavaScript(script) { result, error in
                if result != nil {
                    risultato = processaPreferito(result as! String)
                    tvAlertViewModel.showMessage(risultato.isEmpty ? "Non è stato possibile aggiungere il versetto ai preferiti.": "\(risultato) è stato aggiunto ai preferiti.")
                }
            }
#endif
    }
    
    private func processaPreferito(_ link:String) -> String {
        let store = NSUbiquitousKeyValueStore.default
        var preferitiSalvati: [Preferito] = []
        var risultato = "";
        if link.count == 8 {
            let libro = Int(link[0..<2]) ?? 0
            let capitolo = Int(link[2..<5]) ?? 0
            var versetto = Int(link[5..<8]) ?? 0
            if (versetto==0) { versetto = 1; } // a volte =0 per un capitolo intero, andiamo al primo versetto
            if libro > 0 && capitolo > 0 && versetto > 0 {
                let versione = ContentView.testi.info(item.versione).abbreviazione
                preferitiSalvati = decode(store.string(forKey: "preferiti") ?? "") ?? []
                var inserito = false;
                for index in 0..<preferitiSalvati.count {
                    if libro < Int(preferitiSalvati[index].libro) || (libro == Int(preferitiSalvati[index].libro) && capitolo < Int(preferitiSalvati[index].capitolo)) || (libro == Int(preferitiSalvati[index].libro) && capitolo == Int(preferitiSalvati[index].capitolo) && versetto < Int(preferitiSalvati[index].versetto)) {
                        preferitiSalvati.insert(Preferito(libro: libro, capitolo: capitolo, versetto: versetto, versione: versione), at: index)
                        inserito = true;
                        break;
                    }
                }
                if !inserito {
                    preferitiSalvati.append(Preferito(libro:libro, capitolo:capitolo, versetto:versetto, versione:versione))
                }
                store.set(encode(preferitiSalvati), forKey: "preferiti")
                store.synchronize()
                NotificationCenter.default.post(name: .preferitoCambiatoNotification, object: nil)
                // ContentView.testi.normalizzaRiferimento(model.dataPreferiti[index].libro, model.dataPreferiti[index].capitolo, model.dataPreferiti[index].versetto) + " (" + model.dataPreferiti[index].versione + ")"
                risultato = ContentView.testi.normalizzaRiferimento(libro, capitolo, versetto) + " (" + versione + ")"
            }
        }
        return risultato;
    }
    
    func encode(_ array:[Preferito]) -> String {
        return array.map { "\($0.libro);\($0.capitolo);\($0.versetto);\($0.versione)"}.joined(separator: "/")
    }
    
    func decode(_ string:String) -> [Preferito]? {
        return string.split(separator: "/").compactMap { prefString in
            let components = prefString.split(separator: ";")
            guard components.count == 4,
                  let first = Int(components[0]),
                  let second = Int(components[1]),
                  let third = Int(components[2])
            else { return nil }
            let fourth = String(components[3])
            return Preferito(libro: first, capitolo: second, versetto: third, versione: fourth)
            
        }
    }
    
    private func leggiTesto() {
        if viewModel.voce == StatoTTS.Fermato {
            viewModel.voce = StatoTTS.Inizia
        }
        if viewModel.voce == StatoTTS.InCorso {
            viewModel.voce = StatoTTS.Pausa
        }
        if viewModel.voce == StatoTTS.InPausa {
            viewModel.voce = StatoTTS.Riprendi
        }
        viewModel.voceContinua = sintesiVocaleAutomatico
        viewModel.lingua = ContentView.testi.info(item.versione).lingua
        ricarica(true)
    }
    
    public func spostaTesto(_ libro:Int, _ capitolo:Int, _ versetto:Int) {
        // stesso codice in ContentView:impostaAncora
        var ancoraTemp = (libro <= 9 ? "0" + String(libro) : String(libro));
        let capitoloStringa = "00" + String(capitolo);
        ancoraTemp += capitoloStringa[(capitoloStringa.count - 3)...];
        let versettoStringa = "00" + String(versetto);
        ancoraTemp += versettoStringa[(versettoStringa.count - 3)...];
        viewModel.wvmancora = ancoraTemp
        //tvancora = ancoraTemp
        aggiornaTesto(libro, capitolo)
    }
    
    public func spostaTestoAllaAncora() {
        //print ("TV1: viewModel.ultimaAncora = \(viewModel.ultimaAncora)")
        guard !viewModel.ultimaAncora.isEmpty else { return }

        let script = "document.getElementById('\(viewModel.ultimaAncora)').scrollIntoView({behavior:'smooth'});"
        DispatchQueue.main.asyncAfter(deadline: .now() + 0.2) {
#if os(macOS)
            if let webView = viewModel.coordinatorRef?.wkWebViewParent {
                        webView.evaluateJavaScript(script) { result, error in
                            // Handle error if needed
                        }
                    }
#endif
#if os(iOS)
            viewModel.webView?.evaluateJavaScript(script) { result, error in
                /*
                if let error = error {
                    print("JavaScript error: \(error)")
                } else {
                    print("Scroll script executed")
                }
                 */
            }
#endif
        }/*
        DispatchQueue.main.asyncAfter(deadline: .now() + 0.2) {
            viewModel.webView?.evaluateJavaScript(script, completionHandler: nil)
        }*/

        //print ("TV2: viewModel.ultimaAncora = \(viewModel.ultimaAncora)")
        //viewModel.ultimaAncora = ""
        
        //viewModel.wvmancora = viewModel.ultimaAncora
        //viewModel.ultimaAncora = ""
        //item.generaTesto(true)
    }
    
    private func aggiornaTesto(_ libro:Int, _ capitolo:Int) {
        item.aggiornaTesto(libro, capitolo)
    }
    
    private func riferimentoURLATesto(_ riferimentoURL:String, _ popUp:Bool = true) -> String {
        // lpnb://#220260070000-220260070000#290450180000-290450180000#300040230000-300040230000#410020100000-410020100000?ip=1
        // lpnn://Gen%2B1%3A20%2D25?ip=1
        var riferimento = riferimentoURL.removingPercentEncoding ?? ""
        var versione = popUp ? versioneDaUsare : item.versione
        let protocollo = riferimento[3]
        riferimento = riferimento.remove(0, 7)
        if riferimento.indexOf("?") > -1 {
            riferimento = riferimento[0..<riferimento.indexOf("?")]
        }
        if riferimento.indexOf("##") > -1 {
            versione = riferimento[0..<riferimento.indexOf("##")]
            riferimento = riferimento.remove(0, riferimento.indexOf("##")+2)
        }
        riferimento = riferimento.replacingOccurrences(of: "+", with: " ")
        riferimento = riferimento.replacingOccurrences(of: "_", with: "-")
        
        let t:String
        if protocollo=="b" {
            let rif = ContentView.testi.convertiTitoloNotaARiferimento(riferimento);
            t = ContentView.testi.testoDaRiferimento(rif, versione) //ContentView.testi.testoBrano(rif, versione)
        }
        else {
            t = ContentView.testi.testoBrano(riferimento, item.versione)
        }
        
        return ContentView.testi.mostraHtml(t)
    }
    
    private func coloreLibro(_ i:Int) -> Color {
        if (i <= 21) {
            return (colorScheme == .dark ?  .blue : .cyan)
        }
        if (i <= 28) {
            return (colorScheme == .dark ?  Color(.magenta) : .green)
        }
        if (i <= 46) {
            return (colorScheme == .dark ?  .brown : .orange)
        }
        if (i <= 51) {
            return (colorScheme == .dark ?  Color(.darkGray) : .gray)
        }
        return (colorScheme == .dark ?  .red : .yellow)
    }
}

/*
 #Preview {
 TestoView(Item(versione: "Nuova Riveduta"), "Nuova Riveduta", selection: .constant(Item(versione: "Nuova Riveduta")), tipoSceltaRiferimento: .constant(2))
 }*/
