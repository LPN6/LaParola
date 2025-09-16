//
//  Biblioteca.swift
//  LaParola
//
//  Created by admin on 07/03/24.
//

import SwiftUI

var listaTestiOriginale : [TestoDisponibile] = []
var listaTesti : [TestoDisponibile] = []
var testoAttuale : TestoDisponibile = TestoDisponibile()
var nBibbie : Int = -1

struct Biblioteca: View {
    private var contentView:ContentView
    @State private var selectedButton: String? = nil
    @State private var alertCancella = false
    @State private var alertScarica = false
    @State private var alertMessaggio = ""
    @State private var scaricamentoInCorso = "|"
    @State private var imageHeight: CGFloat = 0
    @Environment(\.modelContext) private var modelContext
    @Environment(\.colorScheme) var colorScheme
    @Binding var refreshFlag: Bool
    
    let colonne = [
        GridItem(.flexible(), spacing:.zero, alignment: .leading),
        GridItem(.fixed(30), spacing:.zero),
        GridItem(.fixed(30), spacing:.zero),
        GridItem(.fixed(30), spacing:.zero)
    ]
    
    @ObservedObject var model = ScaricaModel()
    
    init(_ cvIn:ContentView, refreshFlag: Binding<Bool>) {
        contentView = cvIn
        self._refreshFlag = refreshFlag
    }
    
    var body: some View {
        ScrollView {
            /*
            Text("Biblioteca")
                .padding()
                .font(.title)
             */
            if model.dataTesti.count == 0 {
                Text("Errore: non è stato possibile scaricare da Internet i dati sui testi disponibili.")
            }
            LazyVGrid(columns: colonne) {
                ForEach(model.dataTesti.indices, id: \.self) { index in
                    if index==0 {
                        VStack() {
                            Text("Bibbie")
                                .font(.headline)
                                .frame(minWidth:0, maxWidth: .infinity, alignment: .center)
                                .background(colorScheme == .dark ?  .red : .yellow)
                        }
                            Text(" ")
                            Text(" ")
                            Text(" ")
                    }
                    if index==nBibbie {
                        Text("Commentari")
                            .font(.headline)
                            .frame(minWidth:0, maxWidth: .infinity, alignment: .center)
                            .background(colorScheme == .dark ?  .red : .yellow)
                        Text("")
                        Text("")
                        Text("")
                    }
                    VStack() {
                        GeometryReader { geometry in
                        Text(model.dataTesti[index][0])
                            .frame(height: geometry.size.height)
                            .onAppear {
                                imageHeight = geometry.size.height
                            }
                        }
                        Color.primary.frame(height:1.0)
                    }
                    if model.dataTesti[index][1]=="NS" || model.dataTesti[index][1]=="DA" {
                        if scaricamentoInCorso.contains("|"+model.dataTesti[index][0]+"|") {
                            VStack() {
                                ProgressView()
                                    .frame(height:self.imageHeight)
                                Color.primary.frame(height:1.0)
                            }
                        }
                        else {
                            VStack() {
                                Button(action: {
                                    if (model.dataTesti[index][0] == "Nuova Riveduta" || model.dataTesti[index][0] == "C.E.I.") {
                                        scaricaPredefinita(model.dataTesti[index][0]);
                                        model.creaLista()
                                        refreshFlag.toggle()
                                        do {
                                            try modelContext.save()
                                        } catch {
                                            
                                        }
                                    }
                                    else {
                                        scaricamentoInCorso += model.dataTesti[index][0] + "|"
                                        var urlDaSalvare:URL;
                                        var urlDaScaricare:URL
                                        (urlDaScaricare, urlDaSalvare) = preparaScaricamento(model.dataTesti[index])
                                        Downloader.load(url:urlDaScaricare, to:urlDaSalvare) {messaggioErrore in
                                            scaricamentoInCorso = scaricamentoInCorso.replacingOccurrences(of: "|"+model.dataTesti[index][0]+"|", with: "|");
                                            if messaggioErrore.isEmpty {
                                                ContentView.testi.aggiungiTesto(urlDaSalvare.absoluteString);
                                                model.creaLista()
                                                refreshFlag.toggle()
                                                do {
                                                    try modelContext.save()
                                                } catch {
                                                    
                                                }
                                            }
                                            else {
                                                self.alertMessaggio = messaggioErrore
                                                self.alertScarica = true
                                            }
                                        }
                                    }
                                }) {
                                    Image(systemName: "arrow.down.to.line")
                                        .foregroundStyle(.gray)
                                        .frame(height:self.imageHeight)
                                }
                                .buttonStyle(.borderless)
                                .alert(alertMessaggio, isPresented: $alertScarica) {
                                    //
                                }
                                Color.primary.frame(height:1.0)
                            }
                        }
                    }
                    else {
                        VStack() {
                            Text(" ")
                                .font(.headline)
                                .frame(height:self.imageHeight)
                            Color.primary.frame(height:1.0)
                        }
                    }
                    if model.dataTesti[index][1]=="NS" {
                        VStack() {
                            Button(action: {
                                if (UserDefaults.standard.string(forKey: "testiNascosti") ?? "").isEmpty {
                                    UserDefaults.standard.set(model.dataTesti[index][0], forKey:"testiNascosti")
                                }
                                else {
                                    UserDefaults.standard.set((UserDefaults.standard.string(forKey: "testiNascosti") ?? "") + "|" + model.dataTesti[index][0], forKey:"testiNascosti")
                                }
                                model.creaLista()
                            }) {
                                Image(systemName: "eye.slash")
                                    .foregroundStyle(.green)
                                    .frame(height:self.imageHeight)
                            }
                            .buttonStyle(.borderless)
                            Color.primary.frame(height:1.0)
                        }
                    }
                    else {
                        VStack() {
                            Button(action: {
                                if (ContentView.testi.info(model.dataTesti[index][0]).tipo == TestoTipi.Bibbia && ContentView.testi.nomiVersioni(TestoTipi.Bibbia).count <= 1) {
                                    alertCancella = true
                                }
                                else {
                                    if (model.dataTesti[index][0] == "Nuova Riveduta") {
                                        ContentView.testi.rimuoviTesto(model.dataTesti[index][0]);
                                        UserDefaults.standard.set(true, forKey: "NRCancellata")
                                    }
                                    else {
                                        if (model.dataTesti[index][0] == "C.E.I.") {
                                            ContentView.testi.rimuoviTesto(model.dataTesti[index][0]);
                                            UserDefaults.standard.set(true, forKey: "CEICancellata")
                                        }
                                        else {
                                            _ = ContentView.testi.cancellaTesto(model.dataTesti[index][0]);
                                        }
                                    }
                                    
                                    contentView.rimuoviItemConNome(model.dataTesti[index][0])
                                    
                                    if (model.dataTesti[index][0] == UserDefaults.standard.string(forKey: "versionePreferita")) {
                                        if (ContentView.testi.versioneEsiste("Nuova Riveduta")) {
                                            UserDefaults.standard.set("Nuova Riveduta", forKey: "versionePreferita")
                                        }
                                        else {
                                            if (ContentView.testi.versioneEsiste("C.E.I.")) {
                                                UserDefaults.standard.set("C.E.I.", forKey: "versionePreferita")
                                            }
                                            else {
                                                UserDefaults.standard.set(ContentView.testi.nomiVersioni(TestoTipi.Bibbia)[0], forKey: "versionePreferita")
                                            }
                                        }
                                    }
                                    
                                    model.creaLista()
                                    refreshFlag.toggle()
                                    do {
                                        try modelContext.save()
                                    } catch {
                                        
                                    }
                                }
                            }) {
                                Image(systemName: "trash")
                                    .foregroundStyle(.red)
                                    .frame(height:self.imageHeight)
                            }
                            .buttonStyle(.borderless)
                            .alert("Non è possibile cancellare l'unica versione della Bibbia installata.", isPresented: $alertCancella) {
                                //Button("OK", role: .cancel) {} // non è necessario
                            }
                            Color.primary.frame(height:1.0)
                        }
                    }
                    VStack() {
                        Button(action: {
                            self.selectedButton = model.dataTesti[index][0];
                        }) {
                            Image(systemName: "info")
                                .foregroundStyle(.blue)
                                .frame(height:self.imageHeight)
                        }
                        .popover(isPresented: Binding<Bool>(
                            get: {
                                (index < model.dataTesti.count) && (self.selectedButton == model.dataTesti[index][0])
                            },
                            set: { _ in self.selectedButton = nil }
                        )) {
                            ScrollView {
                                Text(infoTesto(model.dataTesti[index]))
                                    .padding()
                            }
                        }
                        .buttonStyle(.borderless)
                        Color.primary.frame(height:1.0)
                    }
                }
            }
        }
        .navigationTitle("Biblioteca")
    }
    
    func scaricaPredefinita(_ v:String) {
        var resourcesDirectory:String = (Bundle.main.path(forResource:"C.E.I.", ofType:".laparola") ?? "/")
        resourcesDirectory = resourcesDirectory[0..<resourcesDirectory.lastIndexOf("/")]
        let nomeFile = resourcesDirectory + "/" + v + ".laparola"
        
        ContentView.testi.aggiungiTesto(nomeFile);
        if (v == "C.E.I.") {
            UserDefaults.standard.set(false, forKey: "CEICancellata")
        }
        else {
            if (v == "Nuova Riveduta") {
                UserDefaults.standard.set(false, forKey: "NRCancellata")
            }
        }
    }
    
    func preparaScaricamento(_ t:[String]) -> (URL, URL) {
        var stringaDaScaricare = ""
        let urlDaEncodare = t[6] // t[6] = url
        let urlDaEncodarecount = urlDaEncodare.count
        for j in stride(from:0, to:urlDaEncodarecount, by:1) {
            if ((urlDaEncodare[j].first?.unicodeScalars.first!.value)! >= 256)
            {
                // un carattere unicode che non viene tradotto (e così il file non è scaricato)
            }
            else if ((urlDaEncodare[j].first?.unicodeScalars.first!.value)! >= 128) {
                stringaDaScaricare.append("%" + String(format: "%02X", (urlDaEncodare[j].first?.unicodeScalars.first!.value)!));
            }
            else {
                stringaDaScaricare.append(urlDaEncodare[j]);
            }
        }
        stringaDaScaricare = stringaDaScaricare.replacingOccurrences(of: " ", with: "%20")
        
        let urlDaScaricare:URL = URL(string:stringaDaScaricare) ?? URL(fileURLWithPath: "")
        let cartellaDocumenti:URL = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask).first ?? URL(fileURLWithPath: "")
        let urlDaSalvare = cartellaDocumenti.appendingPathComponent(t[0]+".laparola")
        
        if (FileManager.default.fileExists(atPath: urlDaSalvare.path)) {
            let successo = ContentView.testi.cancellaTesto(t[0])
            if !successo {
                do {
                    try FileManager.default.removeItem(at: urlDaSalvare)
                }
                catch {
                    //
                }
            }
        }
        
        return (urlDaScaricare, urlDaSalvare)
    }
}

/* non più necessario perché Biblioteca richiede ContentView, ma non dava una buona preview comunque
 #Preview {
 Biblioteca()
 }
 */

class Downloader {
    class func load(url: URL, to localUrl: URL, completion: @escaping (String) -> ()) {
        let sessionConfig = URLSessionConfiguration.default
        let session = URLSession(configuration: sessionConfig)
        let request = URLRequest(url: url)
        let task = session.downloadTask(with: request) { (tempLocalUrl, response, error) in
            var messaggioErrore = ""
            if let tempLocalUrl = tempLocalUrl, error == nil {
                do {
                    try FileManager.default.copyItem(at: tempLocalUrl, to: localUrl)
                    DispatchQueue.main.async {
                        messaggioErrore = ""
                    }
                } catch (let writeError) {
                    messaggioErrore = String(localized: "Errore nella scrittura del file") + " \(localUrl) : \(writeError)"
                }
            } else {
                messaggioErrore = String(localized: "Errore nello scaricamento del file") + ": \(error?.localizedDescription ?? String(localized: "Nessuna descrizione disponibile"))"
            }
            DispatchQueue.main.async {
                completion(messaggioErrore)
            }
        }
        task.resume()
    }
}

class ScaricaModel: ObservableObject {
    @Published var dataTesti: [[String]] = []
    var datiLetti = false;
    
    init() {
        if !datiLetti {
            self.load("https://www.laparola.net/programma/aggiorna_it.xml")
        }
    }
    
    func load(_ f: String) {
        let url = URL(string: f)!
        let request = URLRequest(url: url)
        
        if listaTestiOriginale.count == 0 {
            let task = URLSession.shared.dataTask(with: request) { (data, response, error) in
                if let error = error {
                    print("Non è stato possibile leggere i dati sui testi disponibili da Internet: \(error)") // in realtà non viene mostrato il messaggio, ma non ha importanza
                    return
                }
                guard let data = data else {
                    print("Non è stato possibile leggere i dati sui testi disponibili da Internet") // in realtà non viene mostrato il messaggio, ma non ha importanza
                    return
                }
                
                let xmlParser = XMLParser(data: data)
                let testiParser = TestiParser()
                xmlParser.delegate = testiParser
                xmlParser.parse()
                DispatchQueue.main.async {
                    self.creaLista();
                    self.datiLetti = true;
                }
            }
            task.resume()
            //DispatchQueue.main.async {
            //    self.creaLista();
            //}
        }
        else {
            DispatchQueue.main.async {
                self.creaLista();
            }
        }
    }
    
    public func creaLista() {
        let versioneApp = Bundle.main.infoDictionary?["CFBundleShortVersionString"] as? String ?? "0.0.0"
        let testiNascostiLista = (UserDefaults.standard.string(forKey: "testiNascosti") ?? "").split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
        self.dataTesti.removeAll()
        listaTesti = listaTestiOriginale
        
        for i in stride(from:0, to:listaTesti.count, by:1) {
            listaTesti[i].versioneAttuale = ContentView.testi.info(listaTesti[i].componente).versione;
            var nomeFile = ContentView.testi.info(listaTesti[i].componente).nomeDelFile;
            if (nomeFile.isEmpty) {
                nomeFile = listaTesti[i].componente + ".laparola";
            }
        }
        
        //var temp:TestoDisponibile
        for i in stride(from:0, to:listaTesti.count, by:1) {
            for j in stride(from:i, to:0, by:-1) {
                if ((!listaTesti[j].versioneAttuale.hasPrefix("0") && listaTesti[j - 1].versioneAttuale.hasPrefix("0")) && ((listaTesti[j].tipo=="Bibbia") == (listaTesti[j - 1].tipo=="Bibbia")))
                {
                    let temp = listaTesti[j];
                    listaTesti[j] = listaTesti[j - 1];
                    listaTesti[j - 1] = temp;
                }
            }
        }
        
        //print(listaTesti.count)
        // questo mette testi italiani in cima - forse se localizzato in English non è necessario, ma forse comunque sì - è sempre un'app per leggere la Bibbia in italiano
        for i in stride(from:1, to:listaTesti.count, by:1) {
            for j in stride(from:i, to:0, by:-1) {
                if (listaTesti[j].lingua == "it" && listaTesti[j - 1].lingua != "it" && ((listaTesti[j].versioneAttuale.hasPrefix("0") && listaTesti[j - 1].versioneAttuale.hasPrefix("0")) || (listaTesti[j].versioneAttuale != "0.0.0" && !listaTesti[j - 1].versioneAttuale.hasPrefix("0"))) && (listaTesti[j].tipo=="Bibbia") == (listaTesti[j - 1].tipo=="Bibbia"))
                {
                    let temp = listaTesti[j];
                    listaTesti[j] = listaTesti[j - 1];
                    listaTesti[j - 1] = temp;
                }
            }
        }
        
        var aggStato:String
        nBibbie = 0
        for i in stride(from:0, to:listaTesti.count, by:1) {
            if listaTesti[i].versioneAttuale.hasPrefix("0") {
                aggStato = "NS" // non scaricato
            }
            else if (confrontaVersioni(listaTesti[i].versioneAttuale, listaTesti[i].versioneNuova, true) < 0 && confrontaVersioni(versioneApp, listaTesti[i].versioneNuova, false) >= 0) {
                aggStato = "DA" // da aggiornare
            }
            else {
                aggStato = "AG" // aggiornato
            }
            
            if !testiNascostiLista.contains(listaTesti[i].componente)  {
                self.dataTesti.append([
                    listaTesti[i].componente, // 0
                    aggStato, // 1
                    listaTesti[i].versioneNuova, // 2
                    listaTesti[i].versioneAttuale, // 3
                    listaTesti[i].tipo, // 4
                    listaTesti[i].dimensione, // 5
                    listaTesti[i].url[0] // 6
                ])
                
                if listaTesti[i].tipo == "Bibbia" {
                    nBibbie += 1
                }
            }
        }
    }
}

private func infoTesto(_ td:[String]) -> String
{
    var messaggio = ""
    if td[3].hasPrefix("0")
    {
        var messTipo = "";
        switch (td[4].lowercased())
        {
        case "bibbia":
            messTipo = String(localized: "questa Bibbia");
            break;
        case "commentario":
            messTipo = String(localized: "questo commentario");
            break;
        case "note":
            messTipo = String(localized: "queste note");
            break;
        default:
            messTipo = String(localized: "questo testo"); // non succede
            break;
        }
        messaggio = String(format: NSLocalizedString("BibliotecaInfoTesto1", comment:""), td[2],messTipo,td[5])
    }
    else
    {
        if (ContentView.testi.info(td[0]).autore != "") {
            messaggio += "\n" + String(localized: "Autore: ") + ContentView.testi.info(td[0]).autore;
        }
        if (ContentView.testi.info(td[0]).data != "") {
            messaggio += "\n" + String(localized: "Data: ") + ContentView.testi.info(td[0]).data;
        }
        if (ContentView.testi.info(td[0]).casaEditrice != "") {
            messaggio += "\n" + String(localized: "Casa editrice: ") + ContentView.testi.info(td[0]).casaEditrice;
        }
        if (ContentView.testi.info(td[0]).isbn != "") {
            messaggio += "\n" + "ISBN: " + ContentView.testi.info(td[0]).isbn;
        }
        if (ContentView.testi.info(td[0]).copyright != "") {
            messaggio += "\n" + "Copyright: " + ContentView.testi.info(td[0]).copyright;
        }
        
        if (confrontaVersioni(td[3], td[2], true) != 0) {
            messaggio += "\n\n" + String(localized: "Versione installata: ") + td[3] + "\n" + String(localized: "Versione disponibile: ") + td[2]
            //messaggio += "\n\nVersione installata: " + td[3] + "\nVersione disponibile: " + td[2];
        }
        
        messaggio += "\n\n" + stripHtml(ContentView.testi.convertiRTF(ContentView.testi.info(td[0]).descrizione, 1))
        while messaggio.hasSuffix("\n") {
            messaggio = messaggio.remove(messaggio.count-1, 1)
        }
        while messaggio.hasPrefix("\n") {
            messaggio = messaggio.remove(0, 1)
        }
    }
    return messaggio
}

private func stripHtml(_ s:String) -> String
{
    var a = s
    a = a.replacingOccurrences(of: "&nbsp;", with: " ")
    a = a.replacingOccurrences(of:"</p>", with:"\n");
    a = a.replacingOccurrences(of:"<br />", with:"\n");
    var n = a.indexOf("<");
    var n1 = 0
    while (n >= 0)
    {
        n1 = a.indexOf(">", n);
        if (n1 < 0) {
            break;
        }
        a = a.remove(n, n1 - n + 1);
        n = a.indexOf("<");
    }
    n = a.indexOf("&#")
    while (n >= 0) {
        n1 = a.indexOf(";", n);
        if (n1 < 0) {
            break;
        }
        a = a[0..<n] + (Unicode.Scalar(Int(a[(n+2)..<n1]) ?? 63)?.description ?? "?") + a[(n1+1)...]
        n = a.indexOf("&#");
    }
    return a;
}

private func confrontaVersioni(_ v1:String, _ v2:String, _ ultimoImportante:Bool) -> Int
{
    let v1a = v1.split(separator:".", omittingEmptySubsequences: true).map{String($0)}
    let v2a = v2.split(separator:".", omittingEmptySubsequences: true).map{String($0)}
    if (v1a.count < 3 || v2a.count < 3) {
        return 0;
    }
    var v1i:Int = Int(v1a[0]) ?? 0
    var v2i:Int = Int(v2a[0]) ?? 0
    if (v1i < v2i) {
        return -1;
    }
    if (v1i > v2i) {
        return 1;
    }
    v1i = Int(v1a[1]) ?? 0
    v2i = Int(v2a[1]) ?? 0
    if (v1i < v2i) {
        return -1;
    }
    if (v1i > v2i) {
        return 1;
    }
    if (!ultimoImportante) {
        return 0;
    }
    v1i = Int(v1a[2]) ?? 0
    v2i = Int(v2a[2]) ?? 0
    if (v1i < v2i) {
        return -1;
    }
    if (v1i > v2i) {
        return 1;
    }
    return 0;
}

class TestiParser : NSObject, XMLParserDelegate {
    var e = ""
    var u = ""
    var isIt = false
    
    func parser(_ parser: XMLParser,
                didStartElement elementName: String,
                namespaceURI: String?,
                qualifiedName qName: String?,
                attributes attributeDict: [String : String] = [:]) {
        if elementName == "file" {
            testoAttuale = TestoDisponibile()
        }
        else {
            e = elementName.lowercased()
            isIt = attributeDict.values.contains("it")
        }
    }
    
    func parser(_ parse: XMLParser, foundCharacters string:String) {
        if (string.trimmingCharacters(in: .whitespacesAndNewlines) != "") {
            switch e {
            case "nome":
                if isIt {
                    testoAttuale.nome = testoAttuale.nome + string
                }
            case "componente":
                testoAttuale.componente = testoAttuale.componente + string // perché un carattere con ASCII>128 viene letto in un string seperato
            case "nomeFile":
                testoAttuale.nomeFile = string
            case "versione":
                testoAttuale.versioneNuova = string
            case "url":
                u = u + string // // perché un carattere con ASCII>128 viene letto in un string seperato
            case "dimensione":
                testoAttuale.dimensione = string
            case "lingua":
                testoAttuale.lingua = string
            case "tipo":
                testoAttuale.tipo = string
            default:
                break
            }
        }
    }
    
    func parser(_ parser: XMLParser,
                didEndElement elementName: String,
                namespaceURI: String?,
                qualifiedName qName: String?) {
        var daAggiungere = true
        if elementName == "file" {
            if !testoAttuale.nome.contains("Doré") && !testoAttuale.nome.lowercased().contains("aiuto biblico") {
                if testoAttuale.tipo == "Bibbia" || testoAttuale.tipo == "commentario" || testoAttuale.tipo == "note" {
                    daAggiungere = true
                    for t in listaTestiOriginale {
                        if t.componente == testoAttuale.componente {
                            daAggiungere = false
                        }
                    }
                    if daAggiungere {
                        testoAttuale.versioneAttuale = ContentView.testi.info(testoAttuale.componente).versione;
                        listaTestiOriginale.append(testoAttuale)
                    }
                }
            }
        }
        if elementName == "url" {
            testoAttuale.url.append(u)
            u = ""
        }
    }
}
