import React, { useState } from 'react';
import './App.css';
import { getDocument } from './services/api';
import DocumentForm from './components/DocumentForm';
import PdfViewer from './components/PdfViewer';
import Message from './components/Message';

function App() {
  const [documentId, setDocumentId] = useState('');
  const [message, setMessage] = useState('');
  const [messageType, setMessageType] = useState(''); // 'success' or 'error'
  const [pdfBase64, setPdfBase64] = useState(null);
  const [loading, setLoading] = useState(false);



   

const handleSubmit = async (idFromUrlOrInput) => {
    //event.preventDefault();
    console.log("documentIDsss:", idFromUrlOrInput);

    const idToUse = idFromUrlOrInput || documentId;
    if (!idToUse) return;

    console.log("idToUse:", idToUse);
    setMessage('');
    setMessageType('');
    setPdfBase64(null);
    setLoading(true);

    try {
      const data = await getDocument(idToUse);
      if(data.status == "ok" && data.base64){
        setPdfBase64(`data:application/pdf;base64,${data.base64}`);
        setMessage(data.mensaje);
        setMessageType('success');
      }
      else{
        setMessage(data.message || 'No se encontro el documento');
        setMessageType('error');
      }
      /*if (data.base64) {
        setPdfBase64(`data:application/pdf;base64,${data.base64}`);
        setMessage('PDF leído correctamente!');
        setMessageType('success');
      } else {
        setMessage(data.message || 'No se encontro el documento');
        setMessageType('error');
      }*/
    } catch (error) {
      setMessage(error.message);
      setMessageType('error');
    } finally {
      setLoading(false);
    }
  };


  // 🟩 Obtener documentID desde la URL al cargar
  useState(() => {
    const params = new URLSearchParams(window.location.search);
    const id = params.get("documentID");

    if (id) {
      setDocumentId(id);
      console.log("documentID:", id);
      handleSubmit(id);
      
    } else {
      console.log("No se encontró documentID en la URL");
    }
  }, []);

  

  return (
    <div className="container mt-5">
      <div className="row">
        <div className="col-md-8 offset-md-2">
          <div className="card">
            <div className="card-header text-center">
            <img src="/logo.png" alt="Logo" className="logo mb-3 " />
              <h4 className="text-center">Verificador de documentos</h4>
            </div>
            <div className="card-body">
              <DocumentForm
                documentId={documentId}
                setDocumentId={setDocumentId}
                handleSubmit={(e) => {
                                      e.preventDefault();
                                      handleSubmit(); // Llama al real
                                    }}
                loading={loading}
              />
              <Message message={message} messageType={messageType} />
            </div>
          </div>
        </div>
      </div>

      {pdfBase64 && <PdfViewer pdfBase64={pdfBase64} />}
    </div>
  );
}

export default App;

