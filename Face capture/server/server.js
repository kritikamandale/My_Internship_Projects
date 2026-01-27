import express from 'express'
import cors from 'cors'
import dotenv from 'dotenv'
import connectDB from './config/db.js'
import authRoutes from './routes/auth.js'

// Load environment variables
dotenv.config()

// Initialize Express app
const app = express()

// Connect to MongoDB
connectDB()

// Middleware
app.use(cors())
app.use(express.json({ limit: '50mb' })) // Increased limit for base64 images
app.use(express.urlencoded({ extended: true, limit: '50mb' }))

// Routes
app.use('/api/auth', authRoutes)

// Health check endpoint
app.get('/', (req, res) => {
  res.json({ 
    message: 'Face Authentication API Server',
    status: 'running',
    endpoints: {
      signup: 'POST /api/auth/signup',
      login: 'POST /api/auth/login'
    }
  })
})

// Error handling middleware
app.use((err, req, res, next) => {
  console.error('Error:', err)
  res.status(err.status || 500).json({
    error: err.message || 'Internal server error'
  })
})

// Start server
const PORT = process.env.PORT || 5000
app.listen(PORT, () => {
  console.log(`🚀 Server running on port ${PORT}`)
  console.log(`📡 API available at http://localhost:${PORT}`)
  console.log(`🔗 Flask service URL: ${process.env.FLASK_SERVICE_URL || 'http://localhost:5001'}`)
})
