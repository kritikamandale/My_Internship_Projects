import { useState } from 'react'
import CameraFeed from './components/CameraFeed'
import AuthPanel from './components/AuthPanel'
import './App.css'

function App() {
  const [cameraRef, setCameraRef] = useState(null)

  return (
    <div className="app-container">
      <header className="app-header">
        <h1>🔐 Face Recognition Authentication</h1>
      </header>

      <div className="main-content">
        <div className="left-panel">
          <CameraFeed onCameraReady={setCameraRef} />
        </div>

        <div className="right-panel">
          <AuthPanel cameraRef={cameraRef} />
        </div>
      </div>
    </div>
  )
}

export default App
