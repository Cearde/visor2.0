import React from 'react';

const DocumentForm = ({ documentId, setDocumentId, handleSubmit, loading }) => {
  return (
    <form onSubmit={handleSubmit}>
      <div className="form-group">
        <input
          type="text"
          className="form-control"
          value={documentId}
          onChange={(e) => setDocumentId(e.target.value)}
          placeholder="Ingrese ID del documento"
          required
        />
      </div>
      <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
        {loading ? (
          <>
            <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
            {' '}Enviando consulta...
          </>
        ) : (
          'Enviar consulta'
        )}
      </button>
    </form>
  );
};

export default DocumentForm;
