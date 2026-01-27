from flask import Flask, request, jsonify
from flask_cors import CORS
import os
import base64
import io
import numpy as np
from PIL import Image
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

# Initialize Flask app
app = Flask(__name__)
CORS(app)

# TODO: Import face recognition library when ready
# Option 1: Using face_recognition library
# import face_recognition

# Option 2: Using deepface
# from deepface import DeepFace

def decode_base64_image(base64_string):
    """
    Decode base64 image string to PIL Image
    """
    try:
        # Remove data:image/jpeg;base64, prefix if present
        if ',' in base64_string:
            base64_string = base64_string.split(',')[1]
        
        # Decode base64
        image_data = base64.b64decode(base64_string)
        
        # Convert to PIL Image
        image = Image.open(io.BytesIO(image_data))
        
        # Convert to RGB if necessary
        if image.mode != 'RGB':
            image = image.convert('RGB')
        
        return image
    except Exception as e:
        print(f"Error decoding image: {e}")
        return None

def extract_face_encoding_placeholder(image):
    """
    PLACEHOLDER: Extract face encoding from image
    
    Replace this with actual face recognition implementation:
    
    Using face_recognition library:
    ----------------------------
    import face_recognition
    
    # Convert PIL Image to numpy array
    image_array = np.array(image)
    
    # Get face encodings
    face_encodings = face_recognition.face_encodings(image_array)
    
    if len(face_encodings) > 0:
        return face_encodings[0]  # Return first face encoding
    return None
    
    Using deepface:
    --------------
    from deepface import DeepFace
    
    # Save image temporarily or convert to array
    image_array = np.array(image)
    
    # Extract embedding
    embedding_objs = DeepFace.represent(
        img_path=image_array,
        model_name='Facenet',  # or 'VGG-Face', 'OpenFace', 'DeepFace', etc.
        enforce_detection=False
    )
    
    if len(embedding_objs) > 0:
        return np.array(embedding_objs[0]['embedding'])
    return None
    """
    
    # Placeholder: Generate a random embedding vector
    # In production, this should be replaced with actual face recognition
    np.random.seed(hash(str(image.size)) % 2**32)
    return np.random.rand(128).tolist()  # 128-dimensional embedding

def compare_faces_placeholder(stored_encodings, new_encodings):
    """
    PLACEHOLDER: Compare face encodings
    
    Replace this with actual face recognition implementation:
    
    Using face_recognition library:
    ----------------------------
    import face_recognition
    
    # Compare each new encoding with stored encodings
    matches = []
    distances = []
    
    for new_enc in new_encodings:
        for stored_enc in stored_encodings:
            # Calculate face distance
            distance = face_recognition.face_distance([stored_enc], new_enc)[0]
            distances.append(distance)
            
            # Check if faces match (distance < 0.6 is typically a match)
            match = distance < 0.6
            matches.append(match)
    
    # Calculate average distance
    avg_distance = np.mean(distances) if distances else 1.0
    
    # Convert distance to similarity score (0 to 1, higher is better)
    similarity_score = 1.0 - avg_distance
    
    # Determine if overall match
    match_percentage = sum(matches) / len(matches) if matches else 0
    is_match = match_percentage > 0.5
    
    return is_match, similarity_score
    
    Using deepface:
    --------------
    from deepface import DeepFace
    
    # Calculate cosine similarity between embeddings
    similarities = []
    
    for new_enc in new_encodings:
        for stored_enc in stored_encodings:
            # Calculate cosine similarity
            similarity = np.dot(new_enc, stored_enc) / (
                np.linalg.norm(new_enc) * np.linalg.norm(stored_enc)
            )
            similarities.append(similarity)
    
    avg_similarity = np.mean(similarities) if similarities else 0
    is_match = avg_similarity > 0.7  # Threshold for match
    
    return is_match, float(avg_similarity)
    """
    
    # Placeholder: Random comparison
    # In production, implement actual face comparison
    score = np.random.uniform(0.65, 0.95)  # Simulate high match
    is_match = score > 0.6
    
    return is_match, float(score)

@app.route('/', methods=['GET'])
def home():
    """Health check endpoint"""
    return jsonify({
        'service': 'Face Recognition Service',
        'status': 'running',
        'endpoints': {
            'enroll': 'POST /enroll',
            'verify': 'POST /verify'
        }
    })

@app.route('/enroll', methods=['POST'])
def enroll():
    """
    Enroll endpoint: Extract face embeddings from images
    
    Request body:
    {
        "images": ["data:image/jpeg;base64,...", ...]
    }
    
    Response:
    {
        "embedding": [0.123, 0.456, ...],
        "success": true
    }
    """
    try:
        data = request.get_json()
        images_base64 = data.get('images', [])
        
        if not images_base64 or len(images_base64) == 0:
            return jsonify({'error': 'No images provided'}), 400
        
        # Extract encodings from all images
        encodings = []
        
        for img_base64 in images_base64:
            image = decode_base64_image(img_base64)
            
            if image is None:
                continue
            
            encoding = extract_face_encoding_placeholder(image)
            
            if encoding is not None:
                encodings.append(encoding)
        
        if len(encodings) == 0:
            return jsonify({'error': 'No faces detected in images'}), 400
        
        # Average the encodings to create a single representation
        avg_encoding = np.mean(encodings, axis=0).tolist()
        
        return jsonify({
            'embedding': avg_encoding,
            'success': True,
            'faces_detected': len(encodings)
        })
        
    except Exception as e:
        print(f"Enroll error: {e}")
        return jsonify({'error': str(e)}), 500

@app.route('/verify', methods=['POST'])
def verify():
    """
    Verify endpoint: Compare stored face data with new images
    
    Request body:
    {
        "storedFaceData": ["data:image/jpeg;base64,...", ...] or [embedding],
        "newImages": ["data:image/jpeg;base64,...", ...]
    }
    
    Response:
    {
        "match": true/false,
        "score": 0.85,
        "success": true
    }
    """
    try:
        data = request.get_json()
        stored_face_data = data.get('storedFaceData', [])
        new_images_base64 = data.get('newImages', [])
        
        if not stored_face_data or not new_images_base64:
            return jsonify({'error': 'Missing stored face data or new images'}), 400
        
        # Extract encodings from stored data
        # (Could be embeddings or base64 images)
        stored_encodings = []
        
        # Check if stored data is embeddings or images
        if isinstance(stored_face_data[0], (list, np.ndarray)):
            # Already embeddings
            stored_encodings = stored_face_data
        elif isinstance(stored_face_data[0], str):
            # Base64 images - need to extract encodings
            for img_base64 in stored_face_data:
                image = decode_base64_image(img_base64)
                if image:
                    encoding = extract_face_encoding_placeholder(image)
                    if encoding is not None:
                        stored_encodings.append(encoding)
        
        # Extract encodings from new images
        new_encodings = []
        
        for img_base64 in new_images_base64:
            image = decode_base64_image(img_base64)
            if image:
                encoding = extract_face_encoding_placeholder(image)
                if encoding is not None:
                    new_encodings.append(encoding)
        
        if len(stored_encodings) == 0 or len(new_encodings) == 0:
            return jsonify({
                'error': 'Unable to detect faces in images',
                'match': False,
                'score': 0.0
            }), 400
        
        # Compare faces
        is_match, score = compare_faces_placeholder(stored_encodings, new_encodings)
        
        return jsonify({
            'match': is_match,
            'score': score,
            'success': True
        })
        
    except Exception as e:
        print(f"Verify error: {e}")
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    port = int(os.getenv('PORT', 5001))
    
    print(f"🔬 Face Recognition Service starting on port {port}")
    print(f"⚠️  NOTE: Currently using PLACEHOLDER face recognition logic")
    print(f"📝 To enable real face recognition, install face_recognition or deepface")
    print(f"   and update the extract_face_encoding_placeholder() function")
    
    app.run(host='0.0.0.0', port=port, debug=True)
