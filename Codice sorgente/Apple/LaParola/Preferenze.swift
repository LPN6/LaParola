//
//  Preferenze.swift
//  LaParola
//
//  Created by admin on 06/03/24.
//

import SwiftUI

struct Preferenze: View {
    @Environment(FormatoTesto.self) var formato : FormatoTesto
    @State private var popoverNascosti = false
    @Environment(\.horizontalSizeClass) var horizontalSizeClass
    @Binding var versionePreferita: String
    @Binding var tipoSceltaRiferimento: Int
    @Binding var schermoSempreAcceso: Bool
    @Binding var pulsantePreferiti: Bool
    @Binding var preferitiNuovoPannello: Bool
    @Binding var pulsanteSintesiVocale: Bool
    @Binding var sintesiVocaleAutomatico: Bool
    
#if os(macOS)
    let fonts = ["Arial", "Futura", "Georgia", "Helvetica", "Times New Roman", "Verdana"]
#endif
#if os(iOS)
    let fonts = ["Arial", "Futura", "Georgia", "Helvetica", "SF Pro", "Times New Roman", "Verdana"]
#endif
    
    var body: some View {
        @Bindable var formato = formato
        ScrollView {
            VStack {
                /*
                 Text("Preferenze")
                 .padding()
                 .font(.title)
                 */
                DisclosureGroup {
                    HStack {
#if os(iOS)
                        Text("Testo:")
                            .padding()
#endif
                        Picker("Testo:", selection:$formato.testoVisualizzato) {
                            Text("Paragrafi").tag(TestoVisualizzato.Paragrafi)
                            Text("Versetti").tag(TestoVisualizzato.Versetti)
                            Text("Nessuno").tag(TestoVisualizzato.Nessuno)
                        }
                        .pickerStyle(.segmented)
                        .onChange(of: formato.testoVisualizzato) {
                            salvaDati()
                        }
                        //.padding()
                    }
                    Toggle("Titoli visualizzati:", isOn: $formato.titoliVisualizzati)
                        .toggleStyle(.switch)
                        .onChange(of: formato.titoliVisualizzati) {
                            salvaDati()
                        }
                    //.padding()
                } label: {
                    Text("Testo biblico")
                        .font(.title2)
                    //.padding()
                }
                
                DisclosureGroup {
                    Text("Parole ricercate:")
                        .font(.title3)
                    if horizontalSizeClass == .compact {
                        VStack {
                            Toggle("Grassetto:", isOn: $formato.fontRicercaGrassetto)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRicercaGrassetto) {
                                    salvaDati()
                                }
                            //.padding()
                            Toggle("Corsivo:", isOn: $formato.fontRicercaCorsivo)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRicercaCorsivo) {
                                    salvaDati()
                                }
                            //.padding()
                            Toggle("Sottolineato:", isOn: $formato.fontRicercaSottolineato)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRicercaSottolineato) {
                                    salvaDati()
                                }
                            //.padding()
                        }
                    }
                    else {
                        HStack {
                            Toggle("Grassetto:", isOn: $formato.fontRicercaGrassetto)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRicercaGrassetto) {
                                    salvaDati()
                                }
                                .padding()
                            Toggle("Corsivo:", isOn: $formato.fontRicercaCorsivo)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRicercaCorsivo) {
                                    salvaDati()
                                }
                                .padding()
                            Toggle("Sottolineato:", isOn: $formato.fontRicercaSottolineato)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRicercaSottolineato) {
                                    salvaDati()
                                }
                                .padding()
                        }
                    }
                    Toggle("Contesto in link:", isOn: $formato.riferimentoContestoRicerche)
                        .toggleStyle(.switch)
                        .onChange(of: formato.riferimentoContestoRicerche) {
                            salvaDati()
                        }
                } label: {
                    Text("Ricerche")
                        .font(.title2)
                }
                
                DisclosureGroup {
                    HStack {
#if os(iOS)
                        Text("Tipo:")
                        // .padding()
#endif
                        Picker("Tipo:", selection:$formato.riferimentoTipo) {
                            Text("1P 5:2,6-7").tag(RiferimentoTipo.DuePunti)
                            Text("1P 5,2.6-7").tag(RiferimentoTipo.Virgola)
                            Text("1P., 5. 2.6-7:").tag(RiferimentoTipo.Citazione)
                        }
                        .pickerStyle(.segmented)
                        .onChange(of: formato.riferimentoTipo) {
                            salvaDati()
                        }
                        //.padding()
                    }
                    HStack {
#if os(iOS)
                        Text("Formato:")
                        //.padding()
#endif
                        Picker("Formato:", selection:$formato.riferimentoFormato) {
                            Text("Intero").tag(RiferimentoFormato.Intero)
                            Text("Abbreviazione").tag(RiferimentoFormato.Abbreviazione)
                            Text("Nessuno").tag(RiferimentoFormato.Nessuno)
                        }
                        .pickerStyle(.segmented)
                        .onChange(of: formato.riferimentoFormato) {
                            salvaDati()
                        }
                        //.padding()
                    }
                    HStack {
#if os(iOS)
                        Text("Posizione:")
                        // .padding()
#endif
                        Picker("Posizione:", selection:$formato.riferimentoPosto) {
                            Text("Prima, stessa riga").tag(RiferimentoPosto.PrimaStessaRiga)
                            Text("Prima, riga diversa").tag(RiferimentoPosto.PrimaRigaDiversa)
                            Text("Dopo").tag(RiferimentoPosto.Dopo)
                        }
                        .pickerStyle(.segmented)
                        .onChange(of: formato.riferimentoPosto) {
                            salvaDati()
                        }
                        //.padding()
                    }
                    if horizontalSizeClass == .compact {
                        VStack {
                            Toggle("Grassetto:", isOn: $formato.fontRiferimentoGrassetto)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRiferimentoGrassetto) {
                                    salvaDati()
                                }
                            //.padding()
                            Toggle("Corsivo:", isOn: $formato.fontRiferimentoCorsivo)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRiferimentoCorsivo) {
                                    salvaDati()
                                }
                            //.padding()
                            Toggle("Sottolineato:", isOn: $formato.fontRiferimentoSottolineato)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRiferimentoSottolineato) {
                                    salvaDati()
                                }
                            //.padding()
                        }
                    }
                    else {
                        HStack {
                            Toggle("Grassetto:", isOn: $formato.fontRiferimentoGrassetto)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRiferimentoGrassetto) {
                                    salvaDati()
                                }
                            //.padding()
                            Toggle("Corsivo:", isOn: $formato.fontRiferimentoCorsivo)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRiferimentoCorsivo) {
                                    salvaDati()
                                }
                            //.padding()
                            Toggle("Sottolineato:", isOn: $formato.fontRiferimentoSottolineato)
                                .toggleStyle(.switch)
                                .onChange(of: formato.fontRiferimentoSottolineato) {
                                    salvaDati()
                                }
                            //.padding()
                        }
                    }
                    Toggle("In apice:", isOn: $formato.riferimentoApice)
                        .toggleStyle(.switch)
                        .onChange(of: formato.riferimentoApice) {
                            salvaDati()
                        }
                    //.padding()
                } label: {
                    Text("Riferimenti")
                        .font(.title2)
                }
                
                DisclosureGroup {
                    VStack {
                        Toggle("Pulsante visibile:", isOn: $pulsanteSintesiVocale)
                            .toggleStyle(.switch)
                        Toggle("Avanzamento automatico:", isOn: $sintesiVocaleAutomatico)
                            .toggleStyle(.switch)
                    }
                }
                label: {
                    Text("Sintesi vocale")
                        .font(.title2)
                }
                
                DisclosureGroup {
                    VStack {
                        Toggle("Pulsante visibile:", isOn: $pulsantePreferiti)
                            .toggleStyle(.switch)
                        Toggle("Apri sempre in un nuovo pannello", isOn: $preferitiNuovoPannello)
                            .toggleStyle(.switch)
                    }
                }
                label: {
                    Text("Preferiti")
                        .font(.title2)
                }
                
                DisclosureGroup {
                    HStack {
#if os(iOS)
                        Text("Scelta riferimenti:")
                            .padding()
#endif
                        Picker("Scelta riferimenti:", selection:$tipoSceltaRiferimento) {
                            Text("Griglie").tag(0)
                            Text("Elenchi").tag(1)
                            Text("Entrambi").tag(2)
                        }
                        .pickerStyle(.segmented)
                        .onChange(of: tipoSceltaRiferimento) {
                            salvaDati()
                        }
                        //.padding()
                    }
#if os(iOS)
                    Toggle("Schermo sempre acceso:", isOn: $schermoSempreAcceso)
                        .toggleStyle(.switch)
                        .onChange(of: schermoSempreAcceso) {
                            UIApplication.shared.isIdleTimerDisabled = schermoSempreAcceso
                        }
#endif
                } label: {
                    Text("Interfaccia")
                        .font(.title2)
                }
                
                DisclosureGroup {
                    HStack {
                        Text("Font:")
                        Picker("", selection: $formato.fontNome) {
                            ForEach(fonts, id: \.self) { font in
                                Text(font).tag(font)
                            }
                        }
                        .pickerStyle(MenuPickerStyle())
                        .onChange(of: formato.fontNome) {
                            salvaDati()
                        }
                    }
                    HStack {
                        Text("Dimensione:")
                        TextField("", value: $formato.fontDimensione, format: .number) // formatter: NumberFormatter())
                            .fixedSize()
#if os(iOS)
                            .keyboardType(.numberPad)
#endif
                            .textFieldStyle(RoundedBorderTextFieldStyle())
                        Slider(value: $formato.fontDimensione, in: ContentView.minDimensioneFont...ContentView.maxDimensioneFont, step: 1)
                            .onChange(of: formato.fontDimensione) {
                                salvaDati()
                            }
                    }
                } label: {
                    Text("Carattere")
                        .font(.title2)
                }
                
                DisclosureGroup {
                    LazyVGrid(columns: [GridItem(.fixed(120)), GridItem(.fixed(120)), GridItem(.fixed(120))]) {
                        Text("Libro nome")
                        Text("Abbreviazione")
                        Text("Abbreviazioni riconosciute")
                        ForEach (1..<74, id:\.self) { nLibro in
                            TextField("", text:$formato.libriNomi[nLibro])
                                .autocorrectionDisabled(/*@START_MENU_TOKEN@*/true/*@END_MENU_TOKEN@*/)
                                .onChange(of: formato.libriNomi[nLibro]) { oldValue, newValue in
                                    if !controllaLibri(newValue) {
                                        formato.libriNomi[nLibro] = oldValue
                                    }
                                    else {
                                        salvaDati()
                                    }
                                }
                            TextField("", text:$formato.libriAbbreviazioniUsate[nLibro])
                                .autocorrectionDisabled(/*@START_MENU_TOKEN@*/true/*@END_MENU_TOKEN@*/)
                                .onChange(of: formato.libriAbbreviazioniUsate[nLibro]) { oldValue, newValue in
                                    if !controllaLibri(newValue) {
                                        formato.libriAbbreviazioniUsate[nLibro] = oldValue
                                    }
                                    else {
                                        salvaDati()
                                    }
                                }
                            TextField("", text:$formato.libriAbbreviazioniRiconosciute[nLibro])
                                .autocorrectionDisabled(/*@START_MENU_TOKEN@*/true/*@END_MENU_TOKEN@*/)
                                .onChange(of: formato.libriAbbreviazioniRiconosciute[nLibro]) { oldValue, newValue in
                                    if !controllaLibri(newValue, true) {
                                        formato.libriAbbreviazioniRiconosciute[nLibro] = oldValue
                                    }
                                    else {
                                        salvaDati()
                                    }
                                    ContentView.testi.creaAbbreviazioniHash()
                                }
                        }
                    }
                    HStack {
                        Button(action: {
                            formato.libriNomi = ContentView.testi.LibriNomiItaliano.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
                            formato.libriAbbreviazioniUsate = ContentView.testi.LibriAbbreviazioniUsateItaliano.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
                            formato.libriAbbreviazioniRiconosciute = ContentView.testi.LibriAbbreviazioniRiconosciuteItaliano.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
                            salvaDati()
                            ContentView.testi.creaAbbreviazioniHash()
                        }) {
                            Text("Predefiniti italiani")
                        }
                        Button(action: {
                            formato.libriNomi = ContentView.testi.LibriNomiInglese.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
                            formato.libriAbbreviazioniUsate = ContentView.testi.LibriAbbreviazioniUsateInglese.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
                            formato.libriAbbreviazioniRiconosciute = ContentView.testi.LibriAbbreviazioniRiconosciuteInglese.split(separator:"|", omittingEmptySubsequences: false).map{String($0)}
                            salvaDati()
                            ContentView.testi.creaAbbreviazioniHash()
                        }) {
                            Text("Predefiniti inglesi")
                        }
                    }
                } label: {
                    Text("Libri")
                        .font(.title2)
                }
                
                DisclosureGroup {
                    HStack {
                        Text("Versione preferita:")
                        Picker("", selection: $versionePreferita) {
                            ForEach(ContentView.testi.nomiVersioni(TestoTipi.Bibbia), id: \.self) { v in
                                Text(v).tag(v)
                            }
                        }
                        .pickerStyle(MenuPickerStyle())
                        .onChange(of: versionePreferita) {
                            ContentView.testi.UltimaBibbia = versionePreferita
                        }
                    }
                    if !(UserDefaults.standard.string(forKey: "testiNascosti") ?? "").isEmpty {
                        Button(action: {
                            UserDefaults.standard.set("", forKey:"testiNascosti")
                            self.popoverNascosti = true
                        }) {
                            Text("Rivela testi nascosti")
                        }
                        //.padding()
                        .popover(isPresented: $popoverNascosti) {
                            Text("Non ci sono più testi nascosti")
                                .padding()
                        }
                    }
                } label: {
                    Text("Testi")
                        .font(.title2)
                }
            }
            .padding()
        }
    }
}

func salvaDati() -> Void {
    let encoder = JSONEncoder()
    if let data = try? encoder.encode(ContentView.testi.formato) {
        ContentView.formatoData = data
    }
}

func controllaLibri(_ s:String, _ virgola:Bool = false) -> Bool {
    let q = virgola ? "[^A-Za-z1-3è, ]" : "[^A-Za-z1-3è ]"
    return s.isEmpty==false && s.range(of: q, options: .regularExpression) == nil
    // in teoria è meglio
    // 1-3 solo all'inizio, spazio solo secondo dopo numero
    // ma siccome controllo dopo ogni carattere piuttosto della fine della modifica
    // il seguente q non permetterebbe aggiungere un numero
    //let q = "^1-3|1-3 |[A-Za-zè]*$"
    //return s.isEmpty==false && s.range(of: q, options: .regularExpression) == s.startIndex..<s.endIndex
}

/*
 #Preview {
 Preferenze(versionePreferita: .constant("Nuova Riveduta"), tipoSceltaRiferimento: .constant(0), schermoSempreAcceso: .constant(false), pulsantePreferiti: .constant(true), pulsanteSintesiVocale: .constant(true), sintesiVocaleAutomatico: .constant(false))
 .environment(FormatoTesto())
 }
 */
