//
//  ContentView.swift
//  LaParola
//
//  Created by admin on 08/01/24.
//

import SwiftUI
import SwiftData

// TODO importante: per mandare a AppStore, bisogna cambiare BundleIdentifier a net.laparola.LaParola o net.laparola.LaParolaMac in LaParola.xcodeproj (sezione Signing) e 2 volte in Info.plist
// per Build per iOS, metti minimum macOS a 14.4 (se si apre l'app iOS su Mac)
// aumenta Version e Build numeri
// fa sì che è in italiano non in inglese
// build for Any iOS and Any Mac
// Guida a https://www.answertopia.com/swiftui/11250/

// TODO
// when swipe or menu change reference, change the number in menu. Also at load?
// download and check crashes from Apple

// TOD2 universal purchase - does it share settings? https://developer.apple.com/help/app-store-connect/create-an-app-record/add-platforms/
// TOD2 cambiare il menu principale - vedi applicationDidFinishLaunching in AppDelegate
// TOD2 stampa e salva testo per Mac
// TOD2 right click in WebView, migliora menu
// TOD2 testo continuo - problema perché cambiando la visualizzazione di un Item, lo stato è perso e ritorna in cima alla WebView
// TOD2 ma se faccio LazyVStack con un TestoView per ogni capitolo o ogni libro?
// TOD2 doppio clic / long press select verse? Maybe context menu?
// TOD2 nel menu popup, pulsante per aprire nel TestoView, come su Android. E con due pannelli?
// TOD2 evidenziatore, salvato in Cloud per quando si cambia Phone, e fra Phone/Pad/Mac
// TOD2 sintesi vocale: opzione per velocità, voce, volume?

//
// see context menu in https://www.answertopia.com/swiftui/building-context-menus-in-swiftui/ and this code
/*
 struct ContentView: View {
 @State private var isPressed = false
 
 var body: some View {
 Text(isPressed ? "Long press detected!" : "Long press me")
 .onLongPressGesture(minimumDuration: 0.5) { // Adjust the duration as needed
 self.isPressed.toggle()
 }
 }
 }
 
 func webView(_ webView: WKWebView, didFinish navigation: WKNavigation!) {
 // Inject JavaScript here to select a sentence
 let jsCode = """
 // Your JavaScript code to select a sentence
 // Example: document.getSelection().toString();
 """
 webView.evaluateJavaScript(jsCode) { result, error in
 if let sentence = result as? String {
 self.parent.sentenceSelected(sentence)
 }
 }
 }
 
 */

class AlertViewModel: ObservableObject {
    @Published var showAlert: Bool = false
    @Published var alertMessage = ""
    
    func showMessage(_ message: String) {
        alertMessage = message
        showAlert = true
    }
}

struct ContentView: View {
    @Environment(\.scenePhase) var scenePhase
    @Environment(\.modelContext) private var modelContext
    @Environment(\.horizontalSizeClass) var horizontalSizeClass
    @Query(sort: \Item.order, order: .forward) private var items: [Item]
    @State private var selection:Item? = nil
    @State private var mostraErroreBiblioteca = false
    @State private var navigare = false
    @State private var refreshFlag = false
    @StateObject private var viewModel = WebViewModel()
    
    @AppStorage("formatosalvato") static var formatoData: Data = Data()
    @AppStorage("versionepreferita") private var versionePreferita: String = ""
    @AppStorage("tiposceltariferimento") private var tipoSceltaRiferimento: Int = 0
    @AppStorage("schermosempreacceso") private var schermoSempreAcceso: Bool = false
    @AppStorage("pulsantesintesivocale") private var pulsanteSintesiVocale: Bool = true
    @AppStorage("pulsantepreferiti") private var pulsantePreferiti: Bool = true
    @AppStorage("sintesivocaleautomatico") private var sintesiVocaleAutomatico: Bool = false
    @AppStorage("preferitinuovopannello") private var preferitiNuovoPannello: Bool = false
    
    static public let minDimensioneFont = 10.0
    static public let maxDimensioneFont = 72.0
    
    static public var testi: Texts = Texts(FormatoTesto())
    
    @StateObject private var alertViewModel = AlertViewModel()
    
    init() {
        let decoder = JSONDecoder()
        if let name = try? decoder.decode(FormatoTesto.self, from: ContentView.formatoData) {
            ContentView.testi.formato = name
        }
        
        var resourcesDirectory:String = (Bundle.main.path(forResource:"C.E.I.", ofType:".laparola") ?? "/")
        resourcesDirectory = resourcesDirectory[0..<resourcesDirectory.lastIndexOf("/")]
        ContentView.testi.aggiungiDirectory(resourcesDirectory)
        
        if (UserDefaults.standard.bool(forKey: "NRCancellata")) {
            ContentView.testi.rimuoviTesto("Nuova Riveduta");
        }
        if (UserDefaults.standard.bool(forKey: "CEICancellata")) {
            ContentView.testi.rimuoviTesto("C.E.I.");
        }
        
        let cartellaDocumenti:URL = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask).first ?? URL(fileURLWithPath: "")
        ContentView.testi.aggiungiDirectory(cartellaDocumenti);
        
        if (ContentView.testi.nomiVersioni(TestoTipi.Bibbia).count == 0) {
            ContentView.testi.aggiungiDirectory(resourcesDirectory);
            UserDefaults.standard.set(false, forKey: "NRCancellata")
            UserDefaults.standard.set(false, forKey: "CEICancellata")
            var s = UserDefaults.standard.string(forKey: "testiNascosti") ?? ""
            s = s.replacingOccurrences(of:"Nuova Riveduta", with:"");
            s = s.replacingOccurrences(of:"C.E.I.", with:"");
            s = s.replacingOccurrences(of:"||", with:"|");
            UserDefaults.standard.set(s, forKey:"testiNascosti")
        }
        
        if versionePreferita.isEmpty {
            if ContentView.testi.versioneEsiste("Nuova Riveduta") {
                versionePreferita = "Nuova Riveduta"
            }
            else {
                if ContentView.testi.versioneEsiste("C.E.I.") {
                    versionePreferita = "C.E.I."
                }
                else {
                    versionePreferita = ContentView.testi.nomiVersioni(TestoTipi.Bibbia)[0]
                }
            }
        }
        ContentView.testi.UltimaBibbia = versionePreferita
#if os(iOS)
        UIApplication.shared.isIdleTimerDisabled = schermoSempreAcceso
#endif
    }
        
    var body: some View {
        NavigationSplitView() {
            List(selection: $selection) {
                ForEach(items, id: \.self) { item in
                    NavigationLink(item.versione, value:item)
                }
                .onDelete(perform: rimuoviItems)
                .onMove(perform: riordinaItems)
                //Divider() //dà un exception di access se si clicca dopo aver aggiunto o rimosso un item, probabilmente un errore in SwiftUI, forse con un aggiornamento funzionerà
                if items.count > 0 {
                    Color.primary.frame(height:1.0)
                }
                else {
                    Text("Premi il pulsante +")
                }
                //Text(items.count>0 ? "——————————————" : "Premi il pulsante +")
                NavigationLink(destination: Preferiti(self)) {
                    Label("Preferiti", systemImage: "star.fill")
                }
                NavigationLink(destination: Biblioteca(self, refreshFlag: $refreshFlag)) {
                    Label("Biblioteca", systemImage: "books.vertical")
                }
                NavigationLink() {
                    Preferenze(versionePreferita: $versionePreferita, tipoSceltaRiferimento: $tipoSceltaRiferimento, schermoSempreAcceso: $schermoSempreAcceso, pulsantePreferiti: $pulsantePreferiti, preferitiNuovoPannello: $preferitiNuovoPannello, pulsanteSintesiVocale: $pulsanteSintesiVocale, sintesiVocaleAutomatico: $sintesiVocaleAutomatico)
                        .onDisappear() {
                            // non più necessario, perché ogni cambiamento viene salvato subito, ma lasciamo qui per essere sicuri
                            let encoder = JSONEncoder()
                            if let data = try? encoder.encode(ContentView.testi.formato) {
                                ContentView.formatoData = data
                            }
                            
                            for i in items {
                                i.generaTesto()
                            }
                        }
                        .navigationTitle("Preferenze")
                } label: {
                    Label("Preferenze", systemImage: "gearshape")
                }
                NavigationLink(destination: Guida()) {
                    Label("Guida", systemImage: "questionmark.circle")
                }
            }
            .navigationTitle("LaParola")
#if os(macOS)
            .navigationSplitViewColumnWidth(min: 230, ideal: 230)
#endif
            //.navigationSplitViewStyle(.automatic)
            .toolbar {
#if os(iOS)
                ToolbarItem(placement: .navigationBarTrailing) {
                    EditButton()
                }
#endif
#if os(macOS)
                ToolbarItem() {
                    Button(action: rimuoviItem) {
                        Label("Rimuovi", systemImage: "minus")
                    }
                }
#endif
                ToolbarItem() {
                    Menu {
                        ForEach(ContentView.testi.nomiVersioni(), id: \.self) { v in
                            Button(action: {
                                //print(selection?.order ?? -1)
                                _ = aggiungiItem(v);
                                //print(selection?.order ?? -1)
                                refreshFlag.toggle()
                            }) {
                                Text(v)
                            }
                        }
                    } label: {
                        Label("Aggiungi", systemImage: "plus")
                    }
                }
                
            }
            .id(refreshFlag)
            .onChange(of: selection) { oldValue, newValue in
                viewModel.voce = StatoTTS.Ferma
                refreshFlag.toggle()
            }
#if os(iOS)
            .onAppear(perform: {
                UIApplication.shared.isIdleTimerDisabled = schermoSempreAcceso
            })
            // = false ora in LaParolaApp, onChange of scenePhase
            //.onDisappear(perform: {
            //    UIApplication.shared.isIdleTimerDisabled = false
            //})
#endif
        } detail: {
            VStack {
                if selection != nil {
                    TestoView(viewModel, selection!, selection!.versione, selection: $selection, tipoSceltaRiferimento: $tipoSceltaRiferimento, pulsantePreferiti: $pulsantePreferiti, pulsanteSintesiVocale: $pulsanteSintesiVocale, sintesiVocaleAutomatico:$sintesiVocaleAutomatico)
                        .navigationTitle(horizontalSizeClass == .compact ? ContentView.testi.info(selection!.versione).abbreviazione : selection!.versione)
                    
                    ForEach(0...(items.count-1), id: \.self) { i in
                        if i < items.count && items[i] == selection { // bisogna controllare i di nuovo, perché a volte se togliamo l'ultimo testo, i = items.count
                            setUltimoItem(i)
                        }
                        else {
                            EmptyView()
                        }
                    }
                } else {
                    if items.count > 0 {
                        let q = UserDefaults.standard.integer(forKey: "ultimoItem") // se non esiste, 0 è predefinito
                        if q >= items.count {
                            TestoView(viewModel, items[0], items[0].versione, selection: $selection, tipoSceltaRiferimento: $tipoSceltaRiferimento, pulsantePreferiti: $pulsantePreferiti, pulsanteSintesiVocale: $pulsanteSintesiVocale, sintesiVocaleAutomatico:$sintesiVocaleAutomatico)
                                .navigationTitle(horizontalSizeClass == .compact ? ContentView.testi.info(items[0].versione).abbreviazione : items[0].versione)
                        }
                        else {
                            TestoView(viewModel, items[q], items[q].versione, selection: $selection, tipoSceltaRiferimento: $tipoSceltaRiferimento, pulsantePreferiti: $pulsantePreferiti, pulsanteSintesiVocale: $pulsanteSintesiVocale, sintesiVocaleAutomatico:$sintesiVocaleAutomatico)
                                .navigationTitle(horizontalSizeClass == .compact ? ContentView.testi.info(items[0].versione).abbreviazione : items[q].versione)
                        }
                    }
                    else {
                        Text("Scegli qualcosa da visualizzare dalla lista a sinistra\no aggiungi un testo con il pulsante +") // questo messaggio probabilmente mai visualizzato, ma lasciato qui caso mai
                            .multilineTextAlignment(.center)
                    }
                }
            }
        }
        .navigationSplitViewStyle(.balanced)
        .environment(ContentView.testi.formato)
#if os(macOS)
        .frame(minWidth: 650, idealWidth: 870)
#endif
        .onAppear {
            if UserDefaults.standard.integer(forKey: "nonPrimaVolta") == 0 {
                _ = aggiungiItem("Nuova Riveduta")
                _ = aggiungiItem("C.E.I.")
                UserDefaults.standard.set(1, forKey: "nonPrimaVolta")
            }
        }
        .alert(isPresented: $alertViewModel.showAlert) {
            Alert(title: Text(""), message: Text(alertViewModel.alertMessage), dismissButton: .default(Text("OK")))
        }
    }
    
    func setUltimoItem(_ i:Int) -> some View {
        UserDefaults.standard.set(i, forKey: "ultimoItem")
        return EmptyView()
    }
    
    private func aggiungiItem(_ v:String) -> Item {
        withAnimation {
            let newItem = Item(versione: v)
            newItem.order = (items.last?.order ?? -1) + 1
            modelContext.insert(newItem)
            if horizontalSizeClass != .compact {
                // con iPhone, il testo non è visibile, solo il menu, per cui non serve impostare quale Item visualizzare
                selection = newItem
            }
            refreshFlag.toggle()
            do {
                try modelContext.save()
            } catch {
                
            }
            return newItem
        }
    }
    
    public func aggiungiPreferito(_ preferito:Preferito) {
        withAnimation {
            var preferitoVersione = ""
            for (nome, versione) in ContentView.testi.versioni {
                if versione.info.abbreviazione == preferito.versione {
                    preferitoVersione = nome
                    break
                }
            }
            if preferitoVersione.isEmpty {
                let messaggio = String(format: NSLocalizedString("PreferitiNonDisponibile", comment:""), preferito.versione)
                alertViewModel.showMessage(messaggio)
            }
            else {
                var trovato = false
                if !preferitiNuovoPannello {
                    for i in items where i.versione == preferitoVersione {
                        impostaAncora(preferito.libro, preferito.capitolo, preferito.versetto)
                        i.spostaTesto(preferito.libro, preferito.capitolo)
                        trovato = true
                        //if horizontalSizeClass != .compact {
                        // con iPhone, il testo non è visibile, solo il menu, per cui non serve impostare quale Item visualizzare
                        selection = i
                        //}
                        break
                    }
                }
                if !trovato {
                    impostaAncora(preferito.libro, preferito.capitolo, preferito.versetto)
                    let newItem = aggiungiItem(preferitoVersione)
                    newItem.spostaTesto(preferito.libro, preferito.capitolo)
                    if horizontalSizeClass == .compact {
                        // con iPhone, il testo non è visibile, solo il menu
                        selection = newItem
                    }
                }
            }
        }
    }
    
    public func impostaAncora(_ libro:Int, _ capitolo:Int, _ versetto:Int) {
        // stesso codice in TestoView:SpostaTesto
        var ancoraTemp = (libro <= 9 ? "0" + String(libro) : String(libro));
        let capitoloStringa = "00" + String(capitolo);
        ancoraTemp += capitoloStringa[(capitoloStringa.count - 3)...];
        let versettoStringa = "00" + String(versetto);
        ancoraTemp += versettoStringa[(versettoStringa.count - 3)...];
        viewModel.wvmancora = ancoraTemp
    }
    
    public func rimuoviItemConNome(_ nome:String) {
        withAnimation {
            for i in self.items {
                if i.versione == nome {
                    modelContext.delete(i)
                    for index in i.order..<items.count {
                        items[index].order -= 1
                    }
                }
            }
            //selection = Set<Item>()
            selection = nil
            do {
                try modelContext.save()
            } catch {
                
            }
        }
    }
    
    private func rimuoviItem() {
        withAnimation {
            /*
             selection.forEach(modelContext.delete)
             selection = Set<Item>()
             */
            if selection != nil {
                modelContext.delete(selection!)
                for index in selection!.order..<items.count {
                    if index >= 0 {
                        items[index].order -= 1
                    }
                }
                selection = nil
                
                //refreshFlag.toggle()
                do {
                    try modelContext.save()
                } catch {
                    
                }
            }
        }
    }
    
    private func rimuoviItems(offsets: IndexSet) {
        withAnimation {
            for index in offsets {
                modelContext.delete(items[index])
                for index2 in items[index].order..<items.count {
                    if index2 >= 0 {
                        items[index2].order -= 1
                    }
                }
            }
            selection = nil // Set<Item>()
            do {
                try modelContext.save()
            }
            catch {
                
            }
        }
    }
    
    private func riordinaItems(from source:IndexSet, to destination: Int) {
        var itemsArray = items
        let offset = destination > source.first! ? -1 : 0
        let itemRiordinato = itemsArray.remove(at: source.first!)
        itemsArray.insert(itemRiordinato, at: destination+offset)
        for index in 0..<itemsArray.count {
            itemsArray[index].order = index
        }
        
        do {
            try modelContext.save()
        }
        catch {
            
        }
    }
}

/*
 #Preview {
 ContentView().environment(FormatoTesto())
 .modelContainer(for: Item.self, inMemory: true)
 }
 */
