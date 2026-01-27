import express from 'express'
import axios from 'axios'
import User from '../models/User.js'

const router = express.Router()

const FLASK_SERVICE_URL = process.env.FLASK_SERVICE_URL || 'http://localhost:5001'

/**
 * @route   POST /api/auth/signup
 * @desc    Register a new user with face data
 * @access  Public
 */
router.post('/signup', async (req, res) => {
  try {
    const { email, username, images } = req.body

    // Validation
    if (!email || !username || !images) {
      return res.status(400).json({ 
        error: 'Email, username, and images are required' 
      })
    }

    if (!Array.isArray(images) || images.length !== 5) {
      return res.status(400).json({ 
        error: 'Exactly 5 face images are required' 
      })
    }

    // Check if user already exists
    const existingUser = await User.findOne({ 
      $or: [{ email }, { username }] 
    })

    if (existingUser) {
      return res.status(400).json({ 
        error: existingUser.email === email 
          ? 'Email already registered' 
          : 'Username already taken' 
      })
    }

    // Optional: Call Flask service to validate images and extract embeddings
    // This can be used to store embeddings instead of raw images for better performance
    try {
      // Uncomment this section when Flask service is ready with real face recognition
      /*
      const flaskResponse = await axios.post(
        `${FLASK_SERVICE_URL}/enroll`,
        { images },
        { timeout: 30000 }
      )
      
      // Store embeddings instead of raw images
      const faceData = flaskResponse.data.embedding || images
      */
      
      // For now, store raw images directly
      const faceData = images
      
      // Create new user
      const newUser = new User({
        email,
        username,
        faceData
      })

      await newUser.save()

      res.status(201).json({ 
        message: 'User registered successfully',
        user: {
          id: newUser._id,
          email: newUser.email,
          username: newUser.username
        }
      })

    } catch (flaskError) {
      console.error('Flask service error:', flaskError.message)
      // If Flask service fails, you can decide whether to:
      // 1. Continue with signup using raw images
      // 2. Return an error to the user
      
      // Option 1: Continue with signup (fallback)
      const newUser = new User({
        email,
        username,
        faceData: images
      })

      await newUser.save()

      res.status(201).json({ 
        message: 'User registered successfully (face verification pending)',
        user: {
          id: newUser._id,
          email: newUser.email,
          username: newUser.username
        }
      })
    }

  } catch (error) {
    console.error('Signup error:', error)
    res.status(500).json({ 
      error: 'Registration failed. Please try again.' 
    })
  }
})

/**
 * @route   POST /api/auth/login
 * @desc    Login user with face recognition
 * @access  Public
 */
router.post('/login', async (req, res) => {
  try {
    const { email, username, images } = req.body

    // Validation
    if (!email || !username || !images) {
      return res.status(400).json({ 
        error: 'Email, username, and images are required' 
      })
    }

    if (!Array.isArray(images) || images.length !== 5) {
      return res.status(400).json({ 
        error: 'Exactly 5 face images are required' 
      })
    }

    // Find user by email and username
    const user = await User.findOne({ email, username })

    if (!user) {
      return res.status(404).json({ 
        error: 'User not found. Please check your credentials.' 
      })
    }

    // Call Flask service to verify face
    try {
      const verifyResponse = await axios.post(
        `${FLASK_SERVICE_URL}/verify`,
        {
          storedFaceData: user.faceData,
          newImages: images
        },
        { timeout: 30000 }
      )

      const { match, score } = verifyResponse.data

      // Define a threshold for face matching
      const MATCH_THRESHOLD = 0.6

      if (match && score >= MATCH_THRESHOLD) {
        // Login successful
        res.json({ 
          message: 'Login successful',
          user: {
            id: user._id,
            email: user.email,
            username: user.username
          },
          matchScore: score
        })
      } else {
        // Face doesn't match
        res.status(401).json({ 
          error: 'Face verification failed. Please try again.',
          matchScore: score
        })
      }

    } catch (flaskError) {
      console.error('Flask service error:', flaskError.message)
      
      // If Flask service is unavailable, return appropriate error
      res.status(503).json({ 
        error: 'Face verification service is currently unavailable. Please try again later.' 
      })
    }

  } catch (error) {
    console.error('Login error:', error)
    res.status(500).json({ 
      error: 'Login failed. Please try again.' 
    })
  }
})

export default router
