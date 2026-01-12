import React from 'react';

const Message = ({ message, messageType }) => {
  if (!message) {
    return null;
  }

  return (
    <div className={`alert alert-${messageType === 'success' ? 'success' : 'danger'} mt-3`} role="alert">
      {message}
    </div>
  );
};

export default Message;
