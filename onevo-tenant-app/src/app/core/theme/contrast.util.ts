export function getContrastRatio(luminance1: number, luminance2: number): number {
  const l1 = Math.max(luminance1, luminance2);
  const l2 = Math.min(luminance1, luminance2);
  return (l1 + 0.05) / (l2 + 0.05);
}

export function hexToRgb(hex: string): { r: number; g: number; b: number } | null {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
  return result
    ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16),
      }
    : null;
}

export function getLuminance(r: number, g: number, b: number): number {
  const a = [r, g, b].map((v) => {
    v /= 255;
    return v <= 0.03928 ? v / 12.92 : Math.pow((v + 0.055) / 1.055, 2.4);
  });
  return a[0] * 0.2126 + a[1] * 0.7152 + a[2] * 0.0722;
}

export function getTextColorForBackground(hexBgColor: string): string {
  const rgb = hexToRgb(hexBgColor);
  if (!rgb) return '#ffffff'; // Fallback to white if invalid hex

  const luminance = getLuminance(rgb.r, rgb.g, rgb.b);
  // White luminance is 1, Black luminance is 0
  const contrastWithWhite = getContrastRatio(1, luminance);
  const contrastWithBlack = getContrastRatio(luminance, 0);

  // WCAG AAA requires a contrast ratio of at least 7:1 for normal text and 4.5:1 for large text.
  // We'll just return whichever has better contrast, leaning towards white if both are okay
  // but if the bg is very light, we need black.
  
  if (contrastWithWhite >= 4.5 || contrastWithWhite > contrastWithBlack) {
    return '#ffffff';
  }
  return '#1e293b'; // Slate 800 instead of pure black for softer look
}
