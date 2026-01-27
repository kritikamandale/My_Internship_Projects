import mongoose from 'mongoose'

const userSchema = new mongoose.Schema({
  email: {
    type: String,
    required: true,
    unique: true,
    lowercase: true,
    trim: true,
    index: true
  },
  username: {
    type: String,
    required: true,
    unique: true,
    trim: true,
    index: true
  },
  faceData: {
    type: [String], // Array of base64 image strings or embeddings
    required: true,
    validate: {
      validator: function(v) {
        return v && v.length === 5
      },
      message: 'Face data must contain exactly 5 images'
    }
  },
  createdAt: {
    type: Date,
    default: Date.now
  }
})

// Compound index for faster lookups during login
userSchema.index({ email: 1, username: 1 })

const User = mongoose.model('User', userSchema)

export default User
