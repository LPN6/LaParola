//
//  AppDelegate.swift
//  LaParola
//
//  Created by admin on 25/05/24.
//

import Foundation

#if os(iOS)
import UIKit
class AppDelegate: NSObject, UIApplicationDelegate {
    func application(_ app: UIApplication, open url: URL, options: [UIApplication.OpenURLOptionsKey : Any] = [:]) -> Bool {
        if url.scheme == "lpnb" || url.scheme == "lpnn" {
            // Handle the URL
            return true
        }
        return false
    }
}

#endif

#if os(macOS)
import Cocoa
import SwiftUI
class AppDelegate: NSObject, NSApplicationDelegate {
    private var aboutBoxWindowController: NSWindowController?
    private var helpWindowController: NSWindowController?
    
    func application(_ application: NSApplication, openFile filename: String) -> Bool {
        if let url = URL(string: filename), url.scheme == "lpnb" || url.scheme == "lpnn" {
            // Handle the URL
            return true
        }
        return false
    }
    
    func applicationDidFinishLaunching(_ notification: Notification) {
        /*
         // nessuno di questi metodi toglie la voce, bisognerebbe aspettare una versione futura di SwiftUI
         in cui è possibile modificare il menu principale
        if let menuItems = NSApp.mainMenu?.items {
            for item in menuItems {
                let q = 2
            }
        }
        if let editMenu = NSApp.mainMenu?.items[2] {
            if let editPastaMenu = editMenu.submenu?.items[5] {
                let qq = 3
                editPastaMenu.isHidden = true
                editMenu.submenu?.removeItem(at: 5)
            }
        }
        
        if let editMenu = NSApp.mainMenu?.item(withTitle:"Edit")?.submenu,
           let itemToHide = editMenu.item(withTitle: "Paste") {
            editMenu.removeItem(itemToHide)
        }
        if let editMenu = NSApp.mainMenu?.item(withTitle:"Modifica")?.submenu,
           let itemToHide = editMenu.item(withTitle: "Incolla") {
            itemToHide.isHidden = true
            editMenu.removeItem(itemToHide)
        }
         */
    }
       
    func showAboutPanel() {
        if aboutBoxWindowController == nil {
            let styleMask: NSWindow.StyleMask = [.closable, .titled]
            let window = NSWindow()
            window.styleMask = styleMask
            window.title = ""
            window.center()
            window.contentView = NSHostingView(rootView: AboutView())
            //window.standardWindowButton(.miniaturizeButton)?.isEnabled = false
            aboutBoxWindowController = NSWindowController(window: window)
        }
        aboutBoxWindowController?.showWindow(aboutBoxWindowController?.window)
    }
    
    func showHelpPanel() {
        if helpWindowController == nil {
            let styleMask: NSWindow.StyleMask = [.closable, .titled]
            let window = NSWindow()
            window.styleMask = styleMask
            window.title = ""
            window.center()
            window.contentView = NSHostingView(rootView: HelpView())
            helpWindowController = NSWindowController(window: window)
        }
        helpWindowController?.showWindow(helpWindowController?.window)
    }
    
    struct AboutView: View {
        var body: some View {
            VStack {
                Image(nsImage: NSApp.applicationIconImage!)
                    .resizable()
                    .frame(width:64, height:64)
                    .padding()
                Text("LaParola")
                    .fontWeight(/*@START_MENU_TOKEN@*/.bold/*@END_MENU_TOKEN@*/)
                if let infoDictionary = Bundle.main.infoDictionary,
                   let versione = infoDictionary["CFBundleShortVersionString"] as? String {
                    Text("Versione \(versione)")
                        .padding()
                }
                Link("https://www.laparola.net/programma/", destination: URL(string: "https://www.laparola.net/programma/")!)
            }
            .frame(minWidth: 300, minHeight: 200)
        }
    }
    
    struct HelpView: View {
        var body: some View {
            VStack {
                Text("Per aiuto sull'utilizzo dell'app LaParola,")
                    .padding()
                Text("scegli 'Guida' in fondo all'elenco dei testi.")
                    .padding()
            }
            .frame(minWidth: 300, minHeight: 200)
        }
    }
}
#endif
