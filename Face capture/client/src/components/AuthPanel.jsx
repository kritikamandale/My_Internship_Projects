import { useState } from 'react'
import SignupForm from './SignupForm'
import LoginForm from './LoginForm'
import './AuthPanel.css'

function AuthPanel({ cameraRef }) {
  const [activeTab, setActiveTab] = useState('signup')

  return (
    <div className="auth-panel">
      <div className="tabs">
        <button
          className={`tab ${activeTab === 'signup' ? 'active' : ''}`}
          onClick={() => setActiveTab('signup')}
        >
          Sign Up
        </button>
        <button
          className={`tab ${activeTab === 'login' ? 'active' : ''}`}
          onClick={() => setActiveTab('login')}
        >
          Login
        </button>
      </div>

      <div className="tab-content">
        {activeTab === 'signup' ? (
          <SignupForm cameraRef={cameraRef} />
        ) : (
          <LoginForm cameraRef={cameraRef} />
        )}
      </div>
    </div>
  )
}

export default AuthPanel
