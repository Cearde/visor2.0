import { API_HOST } from '../config';



export const getDocument = async (documentId) => {
  const response = await fetch(`${API_HOST}/api/sharepoint/datos`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ documentID: documentId }),
  });

  if (!response.ok) {
    const errorData = await response.json();
    console.log("Aqui");
    throw new Error(errorData.message || 'Error al leer el documento desde la apis');
  }

  const data = await response.json();
  return data;
};
