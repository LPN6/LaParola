//
//  LaParolaApp.swift
//  LaParola
//
//  Created by admin on 08/01/24.
//

import SwiftUI
import SwiftData

public extension Character {
    func isLetterOrNumber() -> Bool {
        return self.isLetter || self.isNumber
    }
}

public extension Notification.Name {
    static let preferitoCambiatoNotification = Notification.Name("preferitoCambiatoNotification")
}

public extension String {
    
    func isNumber() -> Bool {
        let c:Character = self.first ?? " "
        return c.isNumber
    }
    
    func isLetterOrNumber() -> Bool {
        let c:Character = self.first ?? " "
        return c.isLetter || c.isNumber
    }
    
    func isLetterGreek() -> Bool {
        let v = (self.first ?? " ").unicodeScalars.first?.value ?? 0
        let r1:ClosedRange<UInt32> = 0x0370...0x03ff
        let r2:ClosedRange<UInt32> = 0x1f00...0x1fff
        return r1.contains(v) || r2.contains(v)
    }
    
    func isLetterHebrew() -> Bool {
        let v = (self.first ?? " ").unicodeScalars.first?.value ?? 0
        let r1:ClosedRange<UInt32> = 0x0591...0x05f4
        let r2:ClosedRange<UInt32> = 0xfb1e...0xfb4f
        return r1.contains(v) || r2.contains(v)
    }
    
    func indexOf(_ s: String) -> Int {
        guard let index = range(of: s)?.lowerBound else { return -1 }
        return distance(from: startIndex, to: index)
    }
    
    func lastIndexOf(_ s: String) -> Int {
        guard let index = range(of: s, options: .backwards)?.lowerBound else { return -1 }
        return distance(from: startIndex, to: index)
    }
    
    func indexOf(_ s: String, _ n: Int) -> Int {
        let q = self[n...].indexOf(s)
        return ((q<0) ? -1 : q+n)
    }
    
    func lastIndexOf(_ s: String, _ n:Int) -> Int {
        //let s1 = self[0..<(n+1)]
        guard let index = self[0..<(n+1)].range(of: s, options: .backwards)?.lowerBound else { return -1 }
        return distance(from: startIndex, to: index)
    }
    
    func indexOfAny(_ a:[String]) -> Int {
        var firstIndex:Int? = nil
        for element in a {
            if let range = range(of:element) {
                let index = distance(from:startIndex, to:range.lowerBound)
                if firstIndex == nil || index<firstIndex! {
                    firstIndex = index
                }
            }
        }
        return firstIndex ?? -1
    }
    
    func indexOfAny(_ a:[String], _ n:Int) -> Int {
        let q = self[n...].indexOfAny(a)
        return ((q<0) ? -1 : q+n)
    }
    
    mutating func insert(_ n:Int, _ s:String) {
        self.insert(contentsOf:s, at:self.index(self.startIndex, offsetBy: n))
    }
    
    subscript(_ range: CountableRange<Int>) -> String {
        let rlb = range.lowerBound
        let start = index(startIndex, offsetBy: max(0, rlb))
        return String(self[start..<index(start, offsetBy: min(self.count, range.upperBound) - rlb)])
    }
    
    subscript(_ range: CountablePartialRangeFrom<Int>) -> String {
        return String(self[index(startIndex, offsetBy: max(0, range.lowerBound))...])
    }
    
    subscript(_ i: Int) -> String {
        return String(self[index(startIndex, offsetBy: i)])
    }
    
    func remove(_ from:Int, _ length:Int) -> String {
        var s = self
        s.removeSubrange(s.index(s.startIndex, offsetBy: from)..<s.index(s.startIndex, offsetBy: length+from))
        return s
    }
    
    func trim() -> String {
        return self.trimmingCharacters(in: .whitespacesAndNewlines)
    }
    
    func trimSuffix() -> String {
        var s = self
        while s.last?.isWhitespace == true {
            s = String(s.dropLast())
        }
        return s
    }
}

extension RandomAccessCollection where Element == String {
    
    func binarySearch(for value: String) -> Index? {
        guard !isEmpty else {
            return nil
        }
        
        let lowercasedValue = value.lowercased().precomposedStringWithCanonicalMapping
        var midStandard = ""
        var mid:Self.Index
        var low = startIndex
        var high = index(before: endIndex)
        
        while low <= high {
            mid = index(low, offsetBy: distance(from: low, to: high) / 2)
            midStandard = self[mid].lowercased().precomposedStringWithCanonicalMapping
            
            if midStandard.localizedStandardCompare(lowercasedValue) == .orderedDescending {
                high = index(mid, offsetBy: -1)
            } else if midStandard.localizedStandardCompare(lowercasedValue) == .orderedAscending {
                low = index(after: mid)
            } else {
                return mid
            }
        }
        
        return nil
    }
}

@main
struct LaParolaApp: App {
#if os(iOS)
    @UIApplicationDelegateAdaptor(AppDelegate.self) var appDelegate
#endif
    
#if os(macOS)
    @NSApplicationDelegateAdaptor(AppDelegate.self) var appDelegate
#endif
        
    @Environment(\.scenePhase) private var scenePhase
    //public static var testi: Texts = Texts()
    //let fileManager = FileManager.default
    
    init() {

    }
    
    @State var sharedModelContainer: ModelContainer = {
        let schema = Schema([
            Item.self,
        ])
        let modelConfiguration = ModelConfiguration(schema: schema, isStoredInMemoryOnly: false)
        
        do {
            return try ModelContainer(for: schema, configurations: [modelConfiguration])
        } catch {
            fatalError("Could not create ModelContainer: \(error)")
        }
    }()
    
    var body: some Scene {
        
        WindowGroup {
            ContentView()
        }
        .modelContainer(sharedModelContainer)
#if os(macOS)
        .commands {
            CommandGroup(replacing: CommandGroupPlacement.appInfo) {
                Button(action: { appDelegate.showAboutPanel() }) {
                    Text("Informazioni su LaParola")
                }
            }
            CommandGroup(replacing: CommandGroupPlacement.undoRedo) {}
            CommandGroup(replacing: .help) {
                Button(action: { appDelegate.showHelpPanel() }) {
                    Text("Aiuto di LaParola")
                }
            }
        }
#endif
        #if os(iOS)
        .onChange(of: scenePhase) { oldScenePhase, newScenePhase in
            switch newScenePhase {
            case .active:
                // viene attivato in ContentView, quando possiamo leggere il valore salvato
                break
            case .background:
                UIApplication.shared.isIdleTimerDisabled = false
            case .inactive:
                UIApplication.shared.isIdleTimerDisabled = false
            @unknown default:
                break
            }
        }
        #endif
    }
}
