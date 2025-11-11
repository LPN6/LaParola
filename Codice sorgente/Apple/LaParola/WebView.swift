//
//  WebView.swift
//  LaParola
//
//  Created by admin on 22/06/24.
//

import SwiftUI
@preconcurrency import WebKit
import AVFoundation

class Coordinator: NSObject, WKNavigationDelegate, AVSpeechSynthesizerDelegate {
    @ObservedObject var viewModel: WebViewModel
    @Binding var anchor: String
    var lastLoadedHtmlTimestamp: Date = .distantPast
    var webViewParent: WebView
    var wkWebViewParent: WKWebView? = nil
    
    init(_ parent: WebView, viewModel: WebViewModel, anchor:Binding<String>) {
        self.viewModel = viewModel
        self._anchor = anchor
        self.webViewParent = parent
        super.init()
        self.viewModel.synthesizer.delegate = self
    }
    
    func webView(_ webView: WKWebView, decidePolicyFor navigationAction: WKNavigationAction, decisionHandler: @escaping (WKNavigationActionPolicy) -> Void) {
        if let urlLink = navigationAction.request.url, urlLink.scheme == "lpnb" || urlLink.scheme == "lpnn" {
            // Handle the URL
            viewModel.clickedURL = urlLink.absoluteString
            if urlLink.scheme == "lpnb" {
                // url = lpnb://#220260070000-220260070000#290450180000-290450180000#300040230000-300040230000#410020100000-410020100000?ip=1
                if urlLink.absoluteString[20] == "_" {
                    viewModel.branoNuovo = true
                    decisionHandler(.allow)
                }
                else {
                    viewModel.branoClicked = true
                    decisionHandler(.cancel)
                }
            }
            else {
                // url = lpnn://Gen%2B1%3A20%2D25?ip=1
                viewModel.notaClicked = true
                decisionHandler(.cancel)
            }
            return
        }
        decisionHandler(.allow)
    }
    
    func webView(_ webView: WKWebView, didFinish navigation: WKNavigation!) {
        wkWebViewParent = webView;
        
        if !anchor.isEmpty {
            viewModel.ultimaAncora = anchor
            anchor = ""
        }
        
        switch self.viewModel.voce {
        case .Inizia:
            /*
            DispatchQueue.main.asyncAfter(deadline: .now() + 0.1) { [weak self] in
                self?.startSpeech(from: webView)
            }
             */
            //webView.evaluateJavaScript("document.body.innerText") { (result, error) in
            DispatchQueue.main.asyncAfter(deadline: .now() + 0.1) { [weak self] in
                guard let self = self else { return }
            webView.evaluateJavaScript("document.body.innerText") {result, error in
                
                guard let text = result as? String else { return }
                DispatchQueue.main.async {
                        //if self.synthesizer.isSpeaking || self.synthesizer.isPaused {
                    self.viewModel.synthesizer.stopSpeaking(at: .immediate)
                        //}
                        let utterance = AVSpeechUtterance(string: text)
                        
                        var voce = AVSpeechSynthesisVoice(language: self.viewModel.lingua)
                        if (voce == nil) {
                            voce = AVSpeechSynthesisVoice(language: nil) // default language for system language
                        }
                        utterance.voice = voce
                    self.viewModel.synthesizer.speak(utterance)
                        self.viewModel.voce = .InCorso
                    }
                }
            }
            //self.viewModel.voce = .InCorso
        case .Ferma:
            DispatchQueue.main.async {
                self.viewModel.synthesizer.stopSpeaking(at: .immediate)
                self.viewModel.voce = .Fermato
            }
        case .Pausa:
            DispatchQueue.main.async {
                self.viewModel.synthesizer.pauseSpeaking(at: .immediate)
                self.viewModel.voce = .InPausa
            }
        case .Riprendi:
            //DispatchQueue.main.async {
            DispatchQueue.main.async {
                self.viewModel.synthesizer.continueSpeaking()
                self.viewModel.voce = .InCorso
            }
            //}
        default:
            break
        }
    }
    
    func speechSynthesizer(_ synthesizer: AVSpeechSynthesizer, didFinish utterance: AVSpeechUtterance) {
        if self.viewModel.voce != .Pausa &&
            self.viewModel.voce != .Ferma &&
            self.viewModel.voce != .Fermato &&
            self.viewModel.voce != .InPausa &&
           // self.viewModel.voce != .InCorso &&
            self.viewModel.voceContinua,
           let webView = wkWebViewParent {

            let script = """
            var link = document.getElementById('prossimocapitolo');
            if (link) { window.location.href = link.href; }
            """

            webView.evaluateJavaScript(script) { [weak self] result, error in
                guard let self = self else { return }

                if error == nil, result != nil {
                    //DispatchQueue.main.asyncAfter(deadline: .now() + 2.2) {
                        self.viewModel.voce = .Inizia
                    //}
                } else {
                    DispatchQueue.main.async {
                        self.viewModel.voce = .Fermato
                    }
                }
            }
        } else {
            DispatchQueue.main.async {
                self.viewModel.voce = .Fermato
            }
        }
    }

}

#if os(macOS)
struct WebView: NSViewRepresentable {
    
    let url: String
    @Binding var anchor: String
    @ObservedObject var viewModel: WebViewModel
    
    func makeCoordinator() -> Coordinator {
        //if let existing = viewModel.coordinatorRef {
          //  return existing
        //}
        let c = Coordinator(self, viewModel: viewModel, anchor: $anchor)
            viewModel.coordinatorRef = c
            return c
    }
      
    func makeNSView(context: Context) -> WKWebView {
        //let webView = viewModel.webView! // WKWebView()
        //webView.navigationDelegate = context.coordinator
        let webView = WKWebView()
            webView.navigationDelegate = context.coordinator
            context.coordinator.wkWebViewParent = webView
        return webView
    }
    
    func updateNSView(_ webView: WKWebView, context: Context) {
        if viewModel.htmlTimestamp > context.coordinator.lastLoadedHtmlTimestamp {
            context.coordinator.lastLoadedHtmlTimestamp = viewModel.htmlTimestamp
            loadHTMLStringAsync(htmlString: url, baseURL: Bundle.main.resourceURL, webView: webView)
        }
        //loadHTMLStringAsync(htmlString: url, baseURL: nil, webView: webView)
        
        // Sync the latest anchor to the viewModel
        if viewModel.ultimaAncora != anchor {
            DispatchQueue.main.async {
                viewModel.ultimaAncora = anchor
            }
        }
    }
    
    func loadHTMLStringAsync(htmlString: String, baseURL: URL?, webView: WKWebView) {
        DispatchQueue.global(qos: .background).async {
            // Perform the I/O operation in the background
            DispatchQueue.main.async {
                // Update the UI on the main thread
                webView.loadHTMLString(htmlString, baseURL: baseURL)
            }
        }
    }
}
#endif

#if os(iOS)
//qqq struct WebView: UIViewRepresentable {
    struct WebView: UIViewControllerRepresentable {
    
    let url: String
    @State private var lastLoadedHtmlTimestamp: Date = .distantPast
    @Binding var anchor: String
    @ObservedObject var viewModel: WebViewModel
    var onSwipe: (CGFloat) -> Void // callback for horinzontal swipe
    
    func makeCoordinator() -> Coordinator {
        Coordinator(self, viewModel: viewModel, anchor: $anchor)
    }
    
        func makeUIViewController(context: Context) -> WebViewController {
            let controller = WebViewController(url: url, viewModel: viewModel, coordinator: context.coordinator)
            
            let swipeGesture = UIPanGestureRecognizer(target: controller, action: #selector(controller.handleHorizontalPan(_:)))
            swipeGesture.delegate = controller // delegate to filter selection handles
            controller.webView.addGestureRecognizer(swipeGesture)
            
            // Pass the swipe callback
                   controller.onSwipe = onSwipe
            
            return controller
        }

        func updateUIViewController(_ webViewController: WebViewController, context: Context) {
            webViewController.updateWebView(url: url)
            // Sync the latest anchor to the viewModel
            if viewModel.ultimaAncora != anchor {
                DispatchQueue.main.async {
                    viewModel.ultimaAncora = anchor
                }
            }
        }
        
    
    func makeUIView(context: Context) -> WKWebView {
            let webView = viewModel.webView! // WKWebView()
        webView.navigationDelegate = context.coordinator
        return webView
    }
    
    func updateUIView(_ webView: WKWebView, context: Context) {
        if viewModel.htmlTimestamp > lastLoadedHtmlTimestamp {
            lastLoadedHtmlTimestamp = viewModel.htmlTimestamp
            loadHTMLStringAsync(htmlString: url, baseURL: Bundle.main.resourceURL, webView: webView)
        }
    }
    
    func loadHTMLStringAsync(htmlString: String, baseURL: URL?, webView: WKWebView) {
        DispatchQueue.global(qos: .background).async {
            // Perform the I/O operation in the background
            DispatchQueue.main.async {
                // Update the UI on the main thread
                webView.loadHTMLString(htmlString, baseURL: baseURL)
            }
        }
    }
}

// Create a ViewController to Manage WKWebView
class WebViewController: UIViewController, UIGestureRecognizerDelegate {
    var webView: WKWebView!
    var viewModel: WebViewModel
    var coordinator: Coordinator
    var lastHTML: String?
    var onSwipe: ((CGFloat) -> Void)? // callback to SwiftUI

    init(url: String, viewModel: WebViewModel, coordinator: Coordinator) {
        self.viewModel = viewModel
        self.coordinator = coordinator
        super.init(nibName: nil, bundle: nil)
        setupWebView()
        loadWebPage(url: url)
    }
    
    @objc func handleHorizontalPan(_ gesture: UIPanGestureRecognizer) {
        guard gesture.state == .ended else { return }
        let translation = gesture.translation(in: webView)
        if abs(translation.x) > abs(translation.y) {
            onSwipe?(translation.x) // call SwiftUI handler
        }
    }
    
    // Prevent swipe from triggering when user moves selection handles
    func gestureRecognizerShouldBegin(_ gestureRecognizer: UIGestureRecognizer) -> Bool {
        // Get the start location of the gesture in the webView
        let location = gestureRecognizer.location(in: webView)
        
        // Find the view under this point
        if let hitView = webView.hitTest(location, with: nil) {
            // Ignore gestures that start on the selection handles
            if String(describing: type(of: hitView)) == "_UITextSelectionHandleView" {
                return false
            }
        }
        
        return true
    }


    required init?(coder: NSCoder) { fatalError("init(coder:) has not been implemented") }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        DispatchQueue.main.async {
            self.viewModel.webView = self.webView
        }
    }
    
    private func setupWebView() {
        webView = WKWebView()
        webView.navigationDelegate = coordinator
        webView.scrollView.delegate = nil // Disable SwiftUI observer conflict
        
        // fully disable horizontal scrolling:
          webView.scrollView.showsHorizontalScrollIndicator = false
          webView.scrollView.alwaysBounceHorizontal = false
          webView.scrollView.bounces = false  // optional: removes elastic bounce
          webView.scrollView.isScrollEnabled = true // still allows vertical scroll

        //viewModel.webView = webView - gives warning, moved to viewDidAppear
        view = webView
    }

    func loadWebPage(url: String) {
        loadHTMLStringAsync(htmlString: url, baseURL: Bundle.main.resourceURL, webView: webView)
        //webView.loadHTMLString(url, baseURL: Bundle.main.resourceURL)
    }

    func updateWebView(url: String) {
        guard lastHTML != url else { return }
                lastHTML = url
        loadHTMLStringAsync(htmlString: url, baseURL: Bundle.main.resourceURL, webView: webView)
        //webView.loadHTMLString(url, baseURL: Bundle.main.resourceURL)
    }
    
    func loadHTMLStringAsync(htmlString: String, baseURL: URL?, webView: WKWebView) {
        DispatchQueue.global(qos: .background).async {
            // Perform the I/O operation in the background
            DispatchQueue.main.async {
                // Update the UI on the main thread
                webView.loadHTMLString(htmlString, baseURL: baseURL)
            }
        }
    }
}
#endif

/*
 struct WebView_Preview: PreviewProvider {
 static var previews: some View {
 WebView(webView: .constant(null), url:"preview", anchor: .constant(""), viewModel: WebViewModel())
 }
 }
 */
