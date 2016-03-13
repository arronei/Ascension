typedef (Uint32Array or sequence<GLuint>) UniformUIVSource;
typedef (Float32Array or sequence<GLfloat>) UniformMatrixFVSource;
typedef (Int32Array or sequence<GLint>) VertexAttribIVSource;
typedef (Uint32Array or sequence<GLuint>) VertexAttribUIVSource;
typedef (Int32Array or sequence<GLint>) ClearBufferIVSource;
typedef (Uint32Array or sequence<GLuint>) ClearBufferUIVSource;
typedef (Float32Array or sequence<GLfloat>) ClearBufferFVSource;

interface WebGL2RenderingContextBase
{
  void deleteTransformFeedback(WebGLTransformFeedback? id);
  [WebGLHandlesContextLoss] GLboolean isTransformFeedback(WebGLTransformFeedback? id);
}