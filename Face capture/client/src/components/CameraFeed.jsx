import { useEffect, useRef, useState } from 'react'
import './CameraFeed.css'

function CameraFeed({ onCameraReady }) {
  const videoRef = useRef(null)
  const canvasRef = useRef(null)
  const [error, setError] = useState(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let stream = null

    const startCamera = async () => {
      try {
        setIsLoading(true)
        stream = await navigator.mediaDevices.getUserMedia({
          video: {
            width: { ideal: 640 },
            height: { ideal: 480 },
            facingMode: 'user'
          }
        })

        if (videoRef.current) {
          videoRef.current.srcObject = stream
          // Pass the video element and canvas to parent for capturing frames
          onCameraReady({
            video: videoRef.current,
            canvas: canvasRef.current
          })
          setError(null)
        }
        setIsLoading(false)
      } catch (err) {
        console.error('Error accessing camera:', err)
        setError('Unable to access camera. Please grant camera permissions.')
        setIsLoading(false)
      }
    }

    startCamera()

    // Cleanup function
    return () => {
      if (stream) {
        stream.getTracks().forEach(track => track.stop())
      }
    }
  }, [onCameraReady])

  return (
    <div className="camera-container">
      <h2>Live Camera Feed</h2>
      
      <div className="video-wrapper">
        {isLoading && (
          <div className="camera-loading">
            <div className="spinner"></div>
            <p>Initializing camera...</p>
          </div>
        )}
        
        {error && (
          <div className="camera-error">
            <p>⚠️ {error}</p>
          </div>
        )}
        
        <video
          ref={videoRef}
          autoPlay
          playsInline
          muted
          className={isLoading || error ? 'hidden' : ''}
        />
        
        {/* Hidden canvas for capturing frames */}
        <canvas ref={canvasRef} style={{ display: 'none' }} />
      </div>

      <div className="camera-info">
        <p>📌 Position your face in the center of the frame</p>
      </div>
    </div>
  )
}

export default CameraFeed
