import { useState, useRef } from 'react'
import axios from 'axios'
import '../components/SignupForm.css'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

function LoginForm({ cameraRef }) {
  const [email, setEmail] = useState('')
  const [username, setUsername] = useState('')
  const [images, setImages] = useState([])
  const [isCapturing, setIsCapturing] = useState(false)
  const [captureCount, setCaptureCount] = useState(0)
  const [statusMessage, setStatusMessage] = useState('')
  const [errorMessage, setErrorMessage] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const intervalRef = useRef(null)

  // Capture a single frame from video
  const captureFrame = () => {
    if (!cameraRef || !cameraRef.video || !cameraRef.canvas) {
      setErrorMessage('Camera not ready. Please wait.')
      return null
    }

    const video = cameraRef.video
    const canvas = cameraRef.canvas

    // Set canvas size to match video
    canvas.width = video.videoWidth
    canvas.height = video.videoHeight

    // Draw video frame to canvas
    const ctx = canvas.getContext('2d')
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height)

    // Convert canvas to base64
    return canvas.toDataURL('image/jpeg', 0.8)
  }

  // Start capturing 5 images at 5-second intervals
  const startCapture = () => {
    if (!cameraRef) {
      setErrorMessage('Camera not initialized. Please refresh the page.')
      return
    }

    setIsCapturing(true)
    setImages([])
    setCaptureCount(0)
    setStatusMessage('Starting capture...')
    setErrorMessage('')

    let count = 0
    const capturedImages = []

    // Capture first image immediately
    const firstImage = captureFrame()
    if (firstImage) {
      capturedImages.push(firstImage)
      setImages([firstImage])
      count = 1
      setCaptureCount(1)
      setStatusMessage(`Captured ${count}/5 photos. Next in 5 seconds...`)
    }

    // Set interval for remaining 4 images
    intervalRef.current = setInterval(() => {
      if (count >= 5) {
        clearInterval(intervalRef.current)
        setIsCapturing(false)
        setStatusMessage('All 5 photos captured! Ready to login.')
        return
      }

      const image = captureFrame()
      if (image) {
        capturedImages.push(image)
        setImages([...capturedImages])
        count++
        setCaptureCount(count)
        
        if (count < 5) {
          setStatusMessage(`Captured ${count}/5 photos. Next in 5 seconds...`)
        } else {
          setStatusMessage('All 5 photos captured! Ready to login.')
          clearInterval(intervalRef.current)
          setIsCapturing(false)
        }
      }
    }, 3000)
  }

  // Cancel capture process
  const cancelCapture = () => {
    if (intervalRef.current) {
      clearInterval(intervalRef.current)
    }
    setIsCapturing(false)
    setStatusMessage('Capture cancelled.')
  }

  // Submit login
  const handleLogin = async (e) => {
    e.preventDefault()
    
    if (!email || !username) {
      setErrorMessage('Please enter email and username.')
      return
    }

    if (images.length !== 5) {
      setErrorMessage('Please capture all 5 photos first.')
      return
    }

    setIsSubmitting(true)
    setErrorMessage('')
    setStatusMessage('Verifying your identity...')

    try {
      const response = await axios.post(`${API_URL}/api/auth/login`, {
        email,
        username,
        images
      })

      setStatusMessage('✅ Login successful! Welcome back.')
      setEmail('')
      setUsername('')
      setImages([])
      setCaptureCount(0)
    } catch (error) {
      console.error('Login error:', error)
      setErrorMessage(
        error.response?.data?.error || 
        'Login failed. Please check your credentials and try again.'
      )
      setStatusMessage('')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="login-form">
      <h2>Login to Your Account</h2>
      
      <form onSubmit={handleLogin}>
        <div className="form-group">
          <label htmlFor="login-email">Email</label>
          <input
            id="login-email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="your.email@example.com"
            disabled={isCapturing || isSubmitting}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="login-username">Username</label>
          <input
            id="login-username"
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Your username"
            disabled={isCapturing || isSubmitting}
            required
          />
        </div>

        <div className="capture-section">
          <h3>Face Verification ({captureCount}/5)</h3>
          
          {!isCapturing && images.length < 5 && (
            <button
              type="button"
              className="btn btn-primary"
              onClick={startCapture}
              disabled={isSubmitting}
            >
              Start Face Capture for Login
            </button>
          )}

          {isCapturing && (
            <button
              type="button"
              className="btn btn-secondary"
              onClick={cancelCapture}
            >
              Cancel Capture
            </button>
          )}

          {images.length === 5 && !isCapturing && (
            <button
              type="submit"
              className="btn btn-success"
              disabled={isSubmitting}
            >
              {isSubmitting ? '⏳ Verifying...' : '🔓 Login with Face'}
            </button>
          )}
        </div>

        {/* Display captured images */}
        <div className="captured-images">
          {images.map((img, index) => (
            <div key={index} className="image-thumbnail">
              <img src={img} alt={`Capture ${index + 1}`} />
              <span className="image-number">{index + 1}</span>
            </div>
          ))}
          {/* Placeholder slots */}
          {[...Array(5 - images.length)].map((_, index) => (
            <div key={`placeholder-${index}`} className="image-placeholder">
              <span>{images.length + index + 1}</span>
            </div>
          ))}
        </div>
      </form>

      {/* Status messages */}
      {statusMessage && (
        <div className="status-message success">
          {statusMessage}
        </div>
      )}

      {errorMessage && (
        <div className="status-message error">
          ⚠️ {errorMessage}
        </div>
      )}
    </div>
  )
}

export default LoginForm
