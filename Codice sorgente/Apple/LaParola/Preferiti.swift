//
//  Preferiti.swift
//  LaParola
//
//  Created by admin on 22/11/24.
//

import SwiftUI

struct Preferiti: View {
    private var contentView:ContentView
    
    let colonne = [
        GridItem(.flexible()),
        GridItem(.flexible(), spacing:.zero, alignment: .leading),
        GridItem(.fixed(30), spacing:.zero),
        GridItem(.flexible())
    ]
    
    @ObservedObject var model = PreferitiModel()
    
    init(_ cvIn:ContentView) {
        contentView = cvIn
    }
    
    var body: some View {
        ScrollView {
            if model.dataPreferiti.count == 0 {
                Text("Non ci sono brani preferiti.\nPer aggiungerne uno, tocca la stella durante la visualizzazione del brano.")
            }
            LazyVGrid(columns: colonne) {
                ForEach(model.dataPreferiti.indices, id: \.self) { index in
                    Text("")
                    Button(action: {
                        apriPreferito(index);
                    })
                    {
                        Text(ContentView.testi.normalizzaRiferimento(model.dataPreferiti[index].libro, model.dataPreferiti[index].capitolo, model.dataPreferiti[index].versetto) + " (" + model.dataPreferiti[index].versione + ")")
                    }
                    .buttonStyle(.borderless)
                    Button(action: {
                        cancellaPreferito(index);
                    })
                    {
                        Image(systemName: "trash")
                            .foregroundStyle(.red)
                    }
                    .buttonStyle(.borderless)
                    Text("")
                }
            }
        }
        .navigationTitle("Preferiti")
    }
    
    func cancellaPreferito(_ index:Int) {
        model.dataPreferiti.remove(at: index);
        model.save()
    }
    
    func apriPreferito(_ index:Int) {
        contentView.aggiungiPreferito(model.dataPreferiti[index]);
    }
}

//#Preview {
//    Preferiti()
//}

struct Preferito
{
    var libro = 1
    var capitolo = 1
    var versetto = 1
    var versione:String = "NR"
}

class PreferitiModel: ObservableObject {
    @Published var dataPreferiti: [Preferito] = []
    let store = NSUbiquitousKeyValueStore.default
    
    init() {
        load()
        NotificationCenter.default.addObserver(self, selector: #selector(storeDidChange(_:)), name: NSUbiquitousKeyValueStore.didChangeExternallyNotification, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(storeDidChange(_:)), name: .preferitoCambiatoNotification, object: nil)
    }
    
    deinit {
        NotificationCenter.default.removeObserver(self, name: NSUbiquitousKeyValueStore.didChangeExternallyNotification, object: nil)
        NotificationCenter.default.removeObserver(self, name: .preferitoCambiatoNotification, object: nil)
    }
    
    func load() {
        let preferitiCaricati = decode(store.string(forKey: "preferiti") ?? "") ?? []
        DispatchQueue.main.async {
            self.dataPreferiti = preferitiCaricati
        }
    }
    
    func save() {
        store.set(encode(dataPreferiti), forKey: "preferiti")
        store.synchronize()
    }
    
    @objc func storeDidChange(_ notification: Notification) {
        load()
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
}

